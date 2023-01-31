using WixSharp.Bootstrapper;
using static WixSharp.Fluent.Redistributables.PayloadExtensions;

namespace WixSharp.Fluent
{
    internal static class WixObjectsPredefined
    {

        /// <summary>
        /// Creates The ExePackage Wix Object With NetFx Payload
        /// 
        /// To obtain Correct RemotePayload:
        /// First, you have to download the ndp48-web.exe and ndp48-x86-x64-allos-enu.exe files 
        /// Then go to wix installation folder
        /// Then run the following commands:
        ///   <code>heat.exe payload [ndp48-web.exe file path] -out [file_name.wxs file path]</code>
        ///   <code>heat.exe payload [ndp48-x86-x64-allos-enu.exe file path] -out [file_name.wxs file path]</code>
        /// 
        /// </summary>
        /// <param name="payload">The File Payload meta-data</param>
        /// <param name="file">The downloaded File Name</param>
        /// <param name="link">Payload download link taken from: https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48</param>
        /// <param name="compressed">Whether the package payload should be embedded in a container or left as an external payload</param>
        /// <param name="id">Unique name for payload usually: NetFxWeb48 or NetFxFull48</param>
        /// <returns></returns>
        internal static ExePackage CreateNetFxPackage(this RemotePayload payload, string link = null, string id = null, string file = null, bool? compressed = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                id = "NetFx";

            var package = new ExePackage
            {
                Id = id,
                Compressed = compressed,
                Cache = true,
                PerMachine = true,
                Vital = true,
                Permanent = true,
                InstallCommand = $"/q /norestart /ChainingPackage \"[WixBundleName]\"",
                RepairCommand = $"/repair /q /norestart /ChainingPackage \"[WixBundleName]\"",
                UninstallCommand = $"/uninstall /q /norestart /ChainingPackage \"[WixBundleName]\"",
            }.HandleRedistributable(payload, file, link);

            return package;
        }

        internal static ExePackage CreateVCppPackage(this RemotePayload payload, string link = null, string id = null, string file = null, bool? compressed = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                id = "VCpp";

            var package = new ExePackage
            {
                Id = id,
                Compressed = compressed,
                Cache = true,
                PerMachine = true,
                Vital = true,
                Permanent = true,
                InstallCommand = $"/q /norestart /ChainingPackage \"[WixBundleName]\"",
                RepairCommand = $"/repair /q /norestart /ChainingPackage \"[WixBundleName]\"",
                UninstallCommand = $"/uninstall /q /norestart /ChainingPackage \"[WixBundleName]\"",
            }.HandleRedistributable(payload, file, link);

            return package;
        }
    }
}
