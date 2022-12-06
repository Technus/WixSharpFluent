using System;
using System.Text;
using WixSharp.Fluent.Attributes;
using WixSharp.Fluent.XML;
using WixSharp;
using WixSharp.CommonTasks;
using DLL = System.Reflection.Assembly;
using static WixSharp.Fluent.Extensions.AssemblyAttributeExtensions;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Extensions for actual Wix Product Projects
    /// </summary>
    public static class WixProjectExtensions
    {
        internal static readonly string elementPlacement = "Wix/Product";
        private static readonly string iconPropName = "ARPPRODUCTICON";
        private static readonly string downgradeErrorMessage = "A newer version is already installed.";

        /// <summary>
        /// Calls:
        /// <see cref="WixCommonExtensions.SetWixDefaults{WixProjectT}(WixProjectT, bool, DLL)"/>
        /// Also:
        ///  * Sets <see cref="MediaTemplate.EmbedCab"/> and clears the <see cref="Project.Media"/>
        ///  * And <see cref="InstallScope.perMachine"/>
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ProjectT SetDefaults<ProjectT>(this ProjectT project,bool noThrow = true, DLL assembly = null) where ProjectT : Project
        {
            project.SetWixDefaults(noThrow: noThrow, assembly: assembly);
            project.SetIdentifiers(noThrow: noThrow, assembly: assembly);
            project.SetIconPath(noThrow: noThrow, assembly: assembly);
            project.SetMajorUpgrade();
            project.SetMediaTemplate();
            project.SetInstallScope();
            return project;
        }

        /// <summary>
        /// Sets Media template to EmbedCab
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <returns></returns>
        public static ProjectT SetMediaTemplate<ProjectT>(this ProjectT project) where ProjectT : Project
        {
            project.Add(new MediaTemplate()
            {
                EmbedCab = true,
            });
            project.Media.Clear();
            return project;
        }

        /// <summary>
        /// Sets install scope of the installer
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="installScope">By default <see cref="InstallScope.perMachine"/></param>
        /// <returns></returns>
        public static ProjectT SetInstallScope<ProjectT>(this ProjectT project, InstallScope? installScope = null) where ProjectT : Project
        {
            project.InstallScope = installScope ?? InstallScope.perMachine;
            return project;
        }

        /// <summary>
        /// Sets the reinstall mode
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="mode">List of modes to concat in resulting mode</param>
        /// <returns></returns>
        public static ProjectT SetReinstallMode<ProjectT>(this ProjectT project, params ReinstallMode[] mode) where ProjectT : Project
        {
            if(mode!=null && mode.Length>0)
            {
                var reinstallMode = mode[0];
                for (int i = 1; i < mode.Length; i++)
                {
                    reinstallMode += mode[i];
                }
                project.ReinstallMode = reinstallMode;
            }
            return project;
        }

        /// <summary>
        /// Sets the Upgrade Code and Guid. Version must be specified it it was not set before.
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="upgradeCode">Upgrade code to use, if not specified will look into assembly</param>
        /// <param name="id">The GUID used to make the package id by default will equal to upgradeCode, to enable consisten ProductID generation set : <see cref="Project.EmitConsistentPackageId"/> </param>
        /// <param name="version">Provide version here if it was not set before</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly">The assembly from which the upgrade code is to be extracted, if not specified use the caller assembly</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">When version was not set before</exception>
        public static ProjectT SetIdentifiers<ProjectT>(this ProjectT project, Guid? upgradeCode = null, Guid? id = null, string version = null, bool noThrow = false, DLL assembly = null) where ProjectT : Project
        {
            project.UpgradeCode = 
                upgradeCode ?? 
                GetAssemblyAttribute<AssemblyProjectUpgradeCodeAttribute>(noThrow, assembly)?.UpgradeCodeGuid ??
                project.UpgradeCode;
            project.GUID = id ?? project.UpgradeCode ?? project.GUID;

            if (!string.IsNullOrEmpty(version))
                project.Version = new Version(version);
            else if (project.Version == null)
                throw new ArgumentException($"The Version must be set before or by this method");

            //project.EmitConsistentPackageId = true;
            project.ProductId = Project.CalculateProductId((Guid)project.GUID, project.Version);
            project.Id = ((Guid)project.ProductId).ToValidWixId();
            return project;
        }

        /// <summary>
        /// Sets the major update behaviour
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="downgradeErrorMessage"></param>
        /// <returns></returns>
        public static ProjectT SetMajorUpgrade<ProjectT>(this ProjectT project, string downgradeErrorMessage = null) where ProjectT : Project
        {
            project.MajorUpgrade = new MajorUpgrade
            {
                DowngradeErrorMessage = downgradeErrorMessage ?? WixProjectExtensions.downgradeErrorMessage,
                Schedule = UpgradeSchedule.afterInstallValidate,
                AllowSameVersionUpgrades = false,
            };
            return project;
        }

        /// <summary>
        /// Sets the icon
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="iconPath">path to icon</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static ProjectT SetIconPath<ProjectT>(this ProjectT project, string iconPath=null, bool noThrow = false, DLL assembly=null) where ProjectT : Project
        {
            iconPath = iconPath ?? GetAssemblyAttribute<AssemblyIconPathAttribute>(noThrow,assembly)?.Path;
            if(iconPath != null)
                project.Properties = project.Properties.Combine(new Property(iconPropName, iconPath));
            return project;
        }

        /// <summary>
        /// Gets the icon path
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string GetIconPath<ProjectT>(this ProjectT project) where ProjectT : Project
        {
            foreach (var prop in project.Properties)
                if (string.Equals(prop.Name, iconPropName))
                    return prop.Value;
            return null;
        }

        /// <summary>
        /// Adds EnsureTable
        /// https://wixtoolset.org/docs/reference/schema/wxs/ensuretable/
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProjectT AddEnsureTable<ProjectT>(this ProjectT project, string name) where ProjectT : Project
        {
            project.AddXmlElement(elementPlacement, "EnsureTable", $"Id={name}");
            return project;
        }

        /// <summary>
        /// Adds the Upgrade Tag
        /// https://wixtoolset.org/docs/reference/schema/wxs/upgrade/
        /// Containing <see cref="UpgradeVersion"/>
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="upgradeCode"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <param name="upgradeVersions">upgrade version definitions</param>
        /// <returns></returns>
        public static ProjectT AddUpgrade<ProjectT>(this ProjectT project, Guid? upgradeCode=null, bool noThrow = false, DLL assembly = null, params UpgradeVersion[] upgradeVersions) where ProjectT : Project
        {
            upgradeCode = upgradeCode ?? project.UpgradeCode ?? project.SetIdentifiers(noThrow: noThrow, assembly: assembly).UpgradeCode;

            StringBuilder xml = new StringBuilder();

            xml.Append($"<Upgrade Id=\"{upgradeCode}\">");
            foreach (var upgradeVersion in upgradeVersions)
                xml.Append(upgradeVersion.ToXml());
            xml.Append("</Upgrade>");

            project.AddXml(elementPlacement, xml.ToString());
            return project;
        }

        /// <summary>
        /// Sets the InstallExecuteSequence
        /// https://wixtoolset.org/docs/reference/schema/wxs/installexecutesequence/
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static ProjectT SetInstallExecuteSequence<ProjectT>(this ProjectT project, string xml) where ProjectT : Project
        {
            project.AddXml($"{elementPlacement}/InstallExecuteSequence", xml);
            return project;
        }

        /// <summary>
        /// Sets the AdminExecuteSequence
        /// https://wixtoolset.org/docs/reference/schema/wxs/adminexecutesequence/
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static ProjectT SetAdminExecuteSequence<ProjectT>(this ProjectT project, string xml) where ProjectT : Project
        {
            project.AddXml($"{elementPlacement}/AdminExecuteSequence", xml);
            return project;
        }

        /// <summary>
        /// Sets the AdvertiseExecuteSequence
        /// https://wixtoolset.org/docs/reference/schema/wxs/advertiseexecutesequence/
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static ProjectT SetAdvertiseExecuteSequence<ProjectT>(this ProjectT project, string xml) where ProjectT : Project
        {
            project.AddXml($"{elementPlacement}/AdvertiseExecuteSequence", xml);
            return project;
        }

        /// <summary>
        /// Adds a fragment in the project root
        /// https://wixtoolset.org/docs/reference/schema/wxs/fragment/
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ProjectT AddFragment<ProjectT>(this ProjectT project, params IXmlAware[] content) where ProjectT : Project
        {
            project.AddWixFragment(elementPlacement, content);
            return project;
        }

        /// <summary>
        /// Adds additional RECURSIVE cleanup logic for a folder (which resits to be removed).
        /// Adds:
        /// <see cref="RegValue"/> with the path
        /// <see cref="Property"/> which looks up with <see cref="RegistrySearch"/> for the <see cref="RegValue"/>
        /// <see cref="UtilRemoveFolderEx"/> wichh looks up the <see cref="Property"/> for the path to clear
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="dirId">directory id</param>
        /// <param name="key">registry key</param>
        /// <param name="name">registry entry name</param>
        /// <param name="productFeature"></param>
        /// <returns></returns>
        public static ProjectT AddRemoveFolderEx<ProjectT>(this ProjectT project, string dirId, string key, string name = "InstallPath", Feature productFeature = null) where ProjectT : Project
        {
            productFeature = productFeature ?? project.DefaultFeature;
            var propName = $"{dirId}InstallPathSearch".EscapeIllegalCharacters().ToUpper();

            project.AddRegValue(new RegValue
            {
                Id = $"{dirId}InstallPath",
                Feature = productFeature,
                Root = RegistryHive.LocalMachine,
                Key = key,
                Name = name,
                Value = $"[{dirId}]",
                Type = "string",
                ForceCreateOnInstall = true,
                ForceDeleteOnUninstall = true,
            });

            project.AddProperty(new Property(propName, 
                new RegistrySearch(RegistryHive.LocalMachine, key, name, RegistrySearchType.raw)
                {
                    Feature= productFeature,
                })
            {
                Feature= productFeature,
            });
            
            project.Add(new UtilRemoveFolderEx()
            {
                Id = $"{dirId}UtilRemoveFolderEx",
                Directory = dirId,
                Feature = productFeature,
                Property = propName,
                On = InstallEvent.uninstall,
                FeatureId= productFeature.Id,
            });

            return project;
        }

        /// <summary>
        /// Adds the smart Feature <see cref="FeatureExtensions.SetSmart{FeatureT}(FeatureT, string, string, int?)"/> mathing property
        /// </summary>
        /// <typeparam name="ProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="feature"></param>
        /// <param name="propertyName"><see cref="FeatureExtensions.GetPropertyName{FeatureT}(FeatureT)"/> Generates default value</param>
        /// <param name="defaultValue">By default "1"</param>
        /// <returns></returns>
        public static ProjectT AddSmartFeatureProperty<ProjectT>(this ProjectT project, Feature feature, string propertyName = null, string defaultValue=null) where ProjectT : Project
        {
            propertyName = propertyName ?? feature.GetPropertyName();
            defaultValue = defaultValue ?? "1";
            project.AddProperty(new Property(propertyName, defaultValue));
            return project;
        }
    }
}
