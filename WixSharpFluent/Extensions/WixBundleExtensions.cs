using System;
using System.Linq;
using WixSharp.Bootstrapper;
using System.IO;
using DLL = System.Reflection.Assembly;
using WixSharp.Fluent.Attributes;
using static WixSharp.Fluent.Extensions.AssemblyAttributeExtensions;
using WixSharp.CommonTasks;
using System.Xml.Linq;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Extensions for Common Bundle operations
    /// </summary>
    public static class WixBundleExtensions
    {
        /// <summary>
        /// The name of variable used to pass installation directory for underlying MSI installation folders.
        /// <see cref="WixCommonExtensions.InstallationFolderId"/>
        /// </summary>
        public static readonly string InstallationFolderVar = "InstallFolder";
        internal static readonly string elementPlacement = "Wix/Bundle";

        /// <summary>
        /// Calls:
        /// <see cref="WixCommonExtensions.SetWixDefaults{WixProjectT}(WixProjectT, bool, DLL)"/>
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static BundleT SetDefaults<BundleT>(this BundleT bundle, bool noThrow = true, DLL assembly = null) where BundleT : Bundle
        {
            bundle.SetWixDefaults(noThrow: noThrow, assembly: assembly);
            bundle.SetIdentifiers(noThrow: noThrow, assembly: assembly);
            bundle.SetApplication(noThrow: noThrow, assembly: assembly);
            bundle.AddInstallFolderVariable(noThrow: noThrow, assembly: assembly);
            return bundle;
        }

        /// <summary>
        /// Sets the <see cref="Bundle"/> values based on <see cref="Project"/> including:
        /// Version,Manufacturer,Name,OoutputFileName,OutputDirectory,WixExtensions, SourceBaseDir,IconFile
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="project"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static BundleT SetFromProject<BundleT>(this BundleT bundle, Project project, bool noThrow = false, DLL assembly = null) where BundleT : Bundle
        {
            bundle.SetVersion(project.Version.ToString(), noThrow: noThrow, assembly: assembly);
            bundle.SetManufacturer(project.ControlPanelInfo.Manufacturer, noThrow: noThrow, assembly: assembly);
            bundle.SetName(project.Name, noThrow: noThrow, assembly: assembly);
            bundle.SetOutFileName(project.OutFileName, noThrow: noThrow, assembly: assembly);
            bundle.SetIconPath(project.GetIconPath(), noThrow: noThrow, assembly: assembly);
            bundle.SetPreserveTempFiles(project.PreserveTempFiles, noThrow: noThrow, assembly: assembly);
            bundle.WixExtensions.AddRange(project.WixExtensions);
            bundle.OutDir = project.OutDir;
            bundle.SourceBaseDir = project.SourceBaseDir;
            bundle.CustomIdAlgorithm = project.CustomIdAlgorithm;

            bundle.Variables = bundle.Variables.Combine(new Variable($"VERSION_MAIN")
            {
                Type = VariableType.version,
                Value = project.Version.ToString(),
            }).ToArray();
            //Looks for installed product version
            bundle.AddFragment(new UtilProductSearch
            {
                UpgradeCode = project.UpgradeCode.ToString(),
                Variable = $"INSTALLED_MAIN",
            });
            return bundle;
        }

        /// <summary>
        /// Sets the icon path
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="iconPath"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static BundleT SetIconPath<BundleT>(this BundleT bundle, string iconPath = null, bool noThrow = false, DLL assembly = null) where BundleT : Bundle
        {
            bundle.IconFile = iconPath ?? GetAssemblyAttribute<AssemblyIconPathAttribute>(noThrow, assembly)?.Path;
            return bundle;
        }

        /// <summary>
        /// Sets the Guid upgrade code
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="upgradeCode">Upgrade code to use, if not specified will look into assembly</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly">The assembly from which the upgrade code is to be extracted, if not specified use the caller assembly</param>
        /// <returns></returns>
        public static BundleT SetIdentifiers<BundleT>(this BundleT bundle, Guid? upgradeCode = null, bool noThrow = false, DLL assembly = null) where BundleT : Bundle
        {
            bundle.UpgradeCode = 
                upgradeCode ??
                GetAssemblyAttribute<AssemblyBundleUpgradeCodeAttribute>(noThrow, assembly)?.UpgradeCodeGuid ?? 
                bundle.UpgradeCode;
            return bundle;
        }

        /// <summary>
        /// Sets the bootstrapper application
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="bootstrapper"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static BundleT SetApplication<BundleT>(this BundleT bundle, WixStandardBootstrapperApplication bootstrapper = null, bool noThrow = false, DLL assembly = null) where BundleT : Bundle
        {
            bundle.Application =
                bootstrapper ??
                GetAssemblyAttribute<AssemblyBootstrapperAttribute>(noThrow, assembly)?.Bootstrapper ??
                bundle.Application;
            return bundle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="link">if HTTPS is not working change to HTTP</param>
        /// <param name="payload"></param>
        /// <param name="detect">Only detects if 'major.minor' version is installed, not detecting will force install</param>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        private static BundleT AddNetFx<BundleT>(this BundleT bundle, string link, RemotePayload payload, bool detect = true, string id = null, string filename = null, bool? compressed = null) where BundleT : Bundle
        {
            //Only relevant for Managed Bootstrapper Applications which this is most likely not
            //bundle.WixVariables.Add("WixMbaPrereqPackageId", "???");
            //bundle.WixVariables.Add("WixMbaPrereqLicenseUrl", "???");
            bundle.Include(WixExtension.NetFx);
            var package = payload.CreateNetFxPackage(link, id, filename, compressed);
            bundle.Chain.Insert(0, package);

            if (detect)
            {
                bundle.AddFragment(
                  new UtilRegistrySearch
                  {
                      Root = RegistryHive.LocalMachine,
                      Key = $@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v{payload.Version.Major}.{payload.Version.Minor}.{payload.Version.Build}\Client",
                      Value = "Version",
                      Variable = "NetFxClientVersionMMB",
                  },
                  new UtilRegistrySearch
                  {
                      Root = RegistryHive.LocalMachine,
                      Key = $@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v{payload.Version.Major}.{payload.Version.Minor}.{payload.Version.Build}\Full",
                      Value = "Version",
                      Variable = "NetFxFullVersionMMB",
                  },
                  new UtilRegistrySearch
                  {
                      Root = RegistryHive.LocalMachine,
                      Key = $@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v{payload.Version.Major}.{payload.Version.Minor}\Client",
                      Value = "Version",
                      Variable = "NetFxClientVersionMM",
                  },
                  new UtilRegistrySearch
                  {
                      Root = RegistryHive.LocalMachine,
                      Key = $@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v{payload.Version.Major}.{payload.Version.Minor}\Full",
                      Value = "Version",
                      Variable = "NetFxFullVersionMM",
                  },
                  new UtilRegistrySearch
                  {
                      Root = RegistryHive.LocalMachine,
                      Key = $@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v{payload.Version.Major}\Client",
                      Value = "Version",
                      Variable = "NetFxClientVersionM",
                  },
                  new UtilRegistrySearch
                  {
                      Root = RegistryHive.LocalMachine,
                      Key = $@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v{payload.Version.Major}\Full",
                      Value = "Version",
                      Variable = "NetFxFullVersionM",
                  }
                );

                bundle.Variables = bundle.Variables.Combine(new Variable[]
                {
          new Variable("NetFxInstallerVersion")
          {
            Value = $"{payload.Version.Major}.{payload.Version.Minor}",
          }
                });

                package.DetectCondition =//According to: https://www.firegiant.com/wix/tutorial/com-expression-syntax-miscellanea/expression-syntax/
                    Condition.Create("NetFxClientVersionMMB << NetFxInstallerVersion") |
                    Condition.Create("NetFxFullVersionMMB << NetFxInstallerVersion") |
                    Condition.Create("NetFxClientVersionMM << NetFxInstallerVersion") |
                    Condition.Create("NetFxFullVersionMM << NetFxInstallerVersion") |
                    Condition.Create("NetFxClientVersionM << NetFxInstallerVersion") |
                    Condition.Create("NetFxFullVersionM << NetFxInstallerVersion");
            }

            return bundle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="link"></param>
        /// <param name="payload"></param>
        /// <param name="detect"></param>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="compressed"></param>
        /// <returns></returns>
        private static BundleT AddVCpp<BundleT>(this BundleT bundle, string link, RemotePayload payload, bool detect = true, string id = null, string filename = null, bool? compressed = null) where BundleT : Bundle
        {
            var package = payload.CreateVCppPackage(link, id, filename, compressed);
            bundle.Chain.Insert(0, package);

            if (detect)
            {
                bundle.AddFragment(
                  new UtilRegistrySearch
                  {
                      Root = RegistryHive.LocalMachine,
                      Key = $@"SOFTWARE\Microsoft\VisualStudio\{payload.Version.Major}.{payload.Version.Minor}\VC\Runtimes\x86",
                      Value = "Version",
                      Variable = "VCppRuntimeVersion",
                  }
                );

                bundle.Variables = bundle.Variables.Combine(new Variable[]
                {
          new Variable("VCppInstallerVersion")
          {
            Value = payload.Version.ToString(),
            Type = VariableType.version,
          }
                });

                package.DetectCondition =//According to: https://www.firegiant.com/wix/tutorial/com-expression-syntax-miscellanea/expression-syntax/
                    Condition.Create("VCppRuntimeVersion >= VCppInstallerVersion");
            }
            return bundle;
        }

        /// <summary>
        /// Adds a remote payload and installation step for .NET 4.8 using Web installer (It will require internet connection when this redist is not present)
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="detect">Only detects if 'major.minor' version is installed, not detecting will force install</param>
        /// <returns></returns>
        public static BundleT AddNetFxWeb48<BundleT>(this BundleT bundle, bool detect = true) where BundleT : Bundle
        {
            return bundle.AddNetFx("https://go.microsoft.com/fwlink/?LinkId=2085155", new RemotePayload
            {
                Hash = "4181398AA1FD5190155AC3A388434E5F7EA0B667",
                Size = 1439328,
                Version = new Version("4.8.4115.0"),
                Description = "Microsoft .NET Framework 4.8 Setup",
                ProductName = "Microsoft .NET Framework 4.8",
                CertificatePublicKey = "F49F9B33E25E33CCA0BFB15A62B7C29FFAB3880B",
                CertificateThumbprint = "ABDCA79AF9DD48A0EA702AD45260B3C03093FB4B",
            }, detect);
        }

        /// <summary>
        /// Bundles Offline .NET 4.8 installer and adds its installation step
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="detect">Only detects if 'major.minor' version is installed, not detecting will force install</param>
        /// <returns></returns>
        public static BundleT AddNetFxFull48<BundleT>(this BundleT bundle, bool detect = true) where BundleT : Bundle
        {
            return bundle.AddNetFx("https://go.microsoft.com/fwlink/?linkid=2088631", new RemotePayload
            {
                Hash = "E322E2E0FB4C86172C38A97DC6C71982134F0570",
                Size = 121307088,
                Version = new Version("4.8.4115.0"),
                Description = "Microsoft .NET Framework 4.8 Setup",
                ProductName = "Microsoft .NET Framework 4.8",
                CertificatePublicKey = "F49F9B33E25E33CCA0BFB15A62B7C29FFAB3880B",
                CertificateThumbprint = "ABDCA79AF9DD48A0EA702AD45260B3C03093FB4B",
            }, detect);
        }

        /// <summary>
        /// Bundles Visual C++ 2012 redistributables and adds its installation step
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="detect"></param>
        /// <returns></returns>
        public static BundleT AddVCpp12<BundleT>(this BundleT bundle, bool detect = true) where BundleT : Bundle
        {
            return bundle.AddVCpp("http://download.visualstudio.microsoft.com/download/pr/10912113/5da66ddebb0ad32ebd4b922fd82e8e25/vcredist_x86.exe", new RemotePayload
            {
                Hash = "0F5D66BCAF120F2D3F340E448A268FE4BBF7709D",
                Size = 6510136,
                Version = new Version("12.0.40664.0"),
                Description = "Microsoft Visual C++ 2013 Redistributable (x86) - 12.0.40664",
                ProductName = "Microsoft Visual C++ 2013 Redistributable (x86) - 12.0.40664",
                CertificatePublicKey = "371DD003A37769487A2A89A5A9DDB3026451B906",
                CertificateThumbprint = "98ED99A67886D020C564923B7DF25E9AC019DF26",
            }, detect);
        }

        /// <summary>
        /// Bundles the Project and adds its installation step
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="project"></param>
        /// <param name="msiPath">Will Build MSI for you if path is null</param>
        /// <param name="preventSameVersionInstall">Use <see cref="UtilProductSearch"/> check to install if same version is present</param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static BundleT AddMsiProject<BundleT>(this BundleT bundle, Project project, string msiPath = null, bool preventSameVersionInstall = false, params string[] properties) where BundleT : Bundle
        {
            msiPath = msiPath ?? project.BuildMsi();

            bundle.Variables = bundle.Variables.Combine(new Variable($"MSI_VERSION_MAIN")
            {
                Type = VariableType.version,
                Value = project.Version.ToString(),
            }).ToArray();

            //Looks for installed product version
            bundle.AddFragment(new UtilProductSearch
            {
                UpgradeCode = project.UpgradeCode.ToString(),
                Variable = $"MSI_INSTALLED_MAIN"
            });

            var msiPackage = new MsiPackage(msiPath)
            {
                MsiProperties = $"{WixCommonExtensions.InstallationFolderId}=[{InstallationFolderVar}]",
            };

            if(properties!=null && properties.Length>0)
                msiPackage.MsiProperties += $"; {properties.JoinBy("; ")}";

            if (preventSameVersionInstall)//this will prevent time wasting on same version install, might cause other issues idk the doc succ
                msiPackage.InstallCondition = $"MSI_VERSION_MAIN > MSI_INSTALLED_MAIN";

            bundle.Chain.Add(msiPackage);
            return bundle;
        }

        /// <summary>
        /// Sets the Default installation folder
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="path"></param>
        /// <param name="prefix">What is the program files folder called defaults to '%ProgramFiles%'</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static BundleT AddInstallFolderVariable<BundleT>(this BundleT bundle, string path=null, string prefix = null, bool noThrow = false, DLL assembly = null) where BundleT : Bundle
        {
            if (prefix != null && path!=null)
                path = Path.Combine(prefix,path);
            else 
                path = path ?? GetInstallationVar(prefix, noThrow, assembly);

            if(path!=null)
            {
                var installFolder = new Variable(InstallationFolderVar, path)
                {
                    Overridable = true
                };//The Wix variable ends with slash anyway
                bundle.Variables = bundle.Variables.Combine(installFolder).ToArray();
            }
            return bundle;
        }

        /// <summary>
        /// Adds Wix Fragment to the Bundle Root.
        /// https://wixtoolset.org/docs/reference/schema/wxs/fragment/
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static BundleT AddFragment<BundleT>(this BundleT bundle, params IXmlAware[] content) where BundleT : Bundle
        {
            bundle.AddWixFragment(elementPlacement, content);
            return bundle;
        }

        /// <summary>
        /// Output the batch file used to build the bundle instead of building it.
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="name"></param>
        /// <param name="pathToRoot">Reverse path from</param>
        /// <returns></returns>
        [Obsolete("Use only to debug commands for EXE generation")]
        public static BundleT BuildBatchFile<BundleT>(this BundleT bundle, string name = null, string pathToRoot = null) where BundleT : Bundle
        {
            name = name ?? Path.GetFullPath(Path.Combine(bundle.OutDir, "Build_" + bundle.OutFileName) + ".cmd");
            pathToRoot = pathToRoot ?? Path.GetFullPath(".");

            bundle.BuildCmd(name);

            System.IO.File.WriteAllText(name, $"CD /d \"{pathToRoot}\"\r\n{System.IO.File.ReadAllText(name)}");//Fix relative paths..

            return bundle;
        }

        /// <summary>
        /// Adds Condition element from Wix Extension bal 
        /// http://schemas.microsoft.com/wix/BalExtension
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="project"></param>
        /// <param name="message"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static BundleT AddCondition<BundleT>(this BundleT project, string message, string condition) where BundleT : Bundle
        {
            var xmlNamespace = WixExtension.Bal.XmlNamespace;
            project.AddXml(elementPlacement, $@"<Condition xmlns=""{xmlNamespace}"" Message=""{message}"">{condition}</Condition>");
            return project;
        }

        /// <summary>
        /// Adds a variable to bundle
        /// </summary>
        /// <typeparam name="BundleT"></typeparam>
        /// <param name="bundle"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static BundleT AddVariable<BundleT>(this BundleT bundle, Variable variable) where BundleT : Bundle
        {
            bundle.Variables = bundle.Variables.Combine(variable).ToArray();
            return bundle;
        }
    }
}
