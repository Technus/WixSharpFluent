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

        internal static ExePackage HandlePayload(this ExePackage package, RemotePayload payload, string filename, string link)
        {
            if (string.IsNullOrWhiteSpace(filename))
                filename = link.GetFileNameFromLink();

            if (package.Compressed == false)
            {
                package.RemotePayloads = new RemotePayload[] { payload };
                package.DownloadUrl = link;
            }
            else
                package.SourceFile = HandleRemotePayloadDownload(payload, filename, link);

            package.Name = $@"{redistFolder}\{filename}";
            return package;
        }
    }
}
