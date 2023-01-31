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

        private static bool HashEquals(Stream fileData, RemotePayload payload, bool noThrow)
        {
            var hash = BitConverter.ToString(SHA1.Create().ComputeHash(fileData)).Replace("-", "");
            var hashOk = string.Equals(hash, payload.Hash, StringComparison.InvariantCultureIgnoreCase);
            if (!noThrow && !hashOk)
                throw new Exception($"Hash invalid, computed: {hash}, required: {payload.Hash}");
            return hashOk;
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

        private static string HandleRemotePayloadDownload(RemotePayload payload, string fileName, string link)
        {
            Directory.CreateDirectory(Path.Combine(DownloadsFolder.DownloadsPath, redistFolder));
            var downloadedFilePath = Path.Combine(DownloadsFolder.DownloadsPath, redistFolder, fileName);

            if (System.IO.File.Exists(downloadedFilePath))
                using (var fileData = System.IO.File.OpenRead(downloadedFilePath))
                    if (HashEquals(fileData, payload, noThrow: true))
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

                try
                {
                    HashEquals(memory, payload, noThrow: false);
                }
                catch(Exception ex)
                {
                    throw new Exception($"Hash Invalid, or download failed: {downloadedFilePath}", ex);
                }

                try
                {
                    using (var file = System.IO.File.OpenWrite(downloadedFilePath))
                    {
                        memory.Seek(0, SeekOrigin.Begin);
                        memory.CopyTo(file);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to write: {downloadedFilePath}", ex);
                }
            }

            return downloadedFilePath;
        }

        private static string HandleLocalPayload(RemotePayload payload, string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                using (var fileData = System.IO.File.OpenRead(filePath))
                {
                    try
                    {
                        HashEquals(fileData, payload, noThrow: false);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Hash Invalid: {filePath}", ex);
                    }
                }
            }
            else throw new Exception($"File does not exist: {filePath}");

            return filePath;
        }

        internal static PackageT HandleRedistributable<PackageT>(this PackageT package, RemotePayload payload, string file, string link) where PackageT : ExePackage
        {
            if (string.IsNullOrWhiteSpace(link) && string.IsNullOrWhiteSpace(file))
                throw new ArgumentException("Both file and link cannot be invalid");

            string name = string.IsNullOrWhiteSpace(file) ? link.GetFileNameFromLink() : file.PathGetFileName();
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Both file and link cannot be resolved to file name");

            package.Compressed = package.Compressed ?? true;
            package.Name = string.IsNullOrWhiteSpace(package.Name) ? $@"{redistFolder}\{name}" : package.Name;
            if (string.IsNullOrWhiteSpace(package.Name))
                throw new Exception("Cannot resolve package name");

            if (package.Compressed ?? true)
                package.SourceFile = string.IsNullOrWhiteSpace(link) 
                    ? HandleLocalPayload(payload, file) 
                    : HandleRemotePayloadDownload(payload, file, link);
            else
            {
                if (string.IsNullOrWhiteSpace(link))
                    throw new ArgumentException("When not including the file, the link must be set", nameof(link));

                package.RemotePayloads = new RemotePayload[] { payload };
                package.DownloadUrl = link;
            }
            return package;
        }
    }
}
