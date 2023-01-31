using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using WixSharp.Bootstrapper;

namespace WixSharp.Fluent.Redistributables
{
    internal static class PayloadExtensions
    {
        private const string redistFolder = "redist";

        private static bool HashEquals(Stream fileData, RemotePayload payload)
        {
            var hash = BitConverter.ToString(SHA1.Create().ComputeHash(fileData)).Replace("-", "");
            return string.Equals(hash, payload.Hash, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string GetFileNameFromLink(this string link)
        {
            using (var client = new WebClient())
            {
                client.OpenRead(link);
                var filename = new ContentDisposition(client.ResponseHeaders["content-disposition"]).FileName;
                if (string.IsNullOrWhiteSpace(filename))
                    throw new Exception("Name could not be obtained");
                return filename;
            }
        }

        private static string HandleRemotePayloadDownload(RemotePayload payload, string filename, string link)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(DownloadsFolder.DownloadsPath, redistFolder));
            var downloadedFilePath = System.IO.Path.Combine(DownloadsFolder.DownloadsPath, redistFolder, filename);

            if (System.IO.File.Exists(downloadedFilePath))
                using (var fileData = System.IO.File.OpenRead(downloadedFilePath))
                    if (HashEquals(fileData, payload))
                        return downloadedFilePath;
                    else System.IO.File.Delete(downloadedFilePath);

            var request = WebRequest.Create(link) as HttpWebRequest;
            request.Timeout = 5000;
            request.Accept = "*/*";

            using (var response = request.GetResponse())
            using (var memory = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(memory);

                memory.Seek(0, SeekOrigin.Begin);
                if (HashEquals(memory, payload))
                    using (var file = System.IO.File.OpenWrite(downloadedFilePath))
                    {
                        memory.Seek(0, SeekOrigin.Begin);
                        memory.CopyTo(file);
                    }
                else throw new Exception($"{filename} Hash Invalid");
            }

            return downloadedFilePath;
        }

        internal static ExePackage HandleRedistributable(this ExePackage package, RemotePayload payload, string file, string link)
        {
            if (string.IsNullOrWhiteSpace(link) && string.IsNullOrWhiteSpace(file))
                throw new ArgumentException("Both file and link cannot be invalid");

            string name = string.IsNullOrWhiteSpace(file) ? link.GetFileNameFromLink() : Path.GetFileName(file);

            package.Compressed = package.Compressed ?? true;
            if (package.Compressed ?? true)
            {
                if (string.IsNullOrWhiteSpace(link))
                {
                    package.SourceFile = file;
                    package.Name = file;
                }
                else
                {
                    package.SourceFile = HandleRemotePayloadDownload(payload, file, link);
                    package.Name = $@"{redistFolder}\{name}";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(link))
                    throw new ArgumentException("When not including the file, the link must be set", nameof(link));

                package.RemotePayloads = new RemotePayload[] { payload };
                package.DownloadUrl = link;
                package.Name = $@"{redistFolder}\{name}";
            }

            return package;
        }
    }
}
