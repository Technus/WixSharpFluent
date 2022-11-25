using System;
using WixSharp.Bootstrapper;
using System.Diagnostics;
using System.Reflection;
using DLL = System.Reflection.Assembly;
using WixSharp.Fluent.Attributes;
using System.Collections.Generic;
using static WixSharp.Fluent.Extensions.AssemblyAttributeExtensions;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Extensions for All types of Wix Projects
    /// </summary>
    public static class WixCommonExtensions
    {
        internal static string wixOutputFolder = "wix";

        static WixCommonExtensions()
        {
            GuidDictionary.Initialize();
            Compiler.AllowNonRtfLicense = true;//Fine since only Hyperlink is allowed anyway
            MSBuild.EmitAutoGenFiles = false;
        }

        /// <summary>
        /// Sets Manufacturer, Includes Extensions and makes the Temp Files preserved for debugging purposes.
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static WixProjectT SetWixDefaults<WixProjectT>(this WixProjectT project, bool noThrow = true, DLL assembly = null) where WixProjectT : WixProject
        {
            project.SetManufacturer(noThrow: noThrow, assembly: assembly);
            project.SetVersion(noThrow: noThrow, assembly: assembly);
            project.SetName(noThrow: noThrow, assembly: assembly);
            project.SetOutFileName(noThrow: noThrow, assembly: assembly);
            project.SetPreserveTempFiles(noThrow: noThrow, assembly: assembly);
            project.AddExtensions(WixExtension.Bal,WixExtension.Util);
            project.PreserveDbgFiles = false;
            project.OutDir = wixOutputFolder;
            return project;
        }

        /// <summary>
        /// Adds the list of extensions to project
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static WixProjectT AddExtensions<WixProjectT>(this WixProjectT project,params WixExtension[] extensions) where WixProjectT : WixProject
        {
            foreach (var extension in extensions)
            {
                project.Include(extension);
            }
            return project;
        }

        /// <summary>
        /// Sets if the temporary files should be preserved in output folder
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="preserveTempFiles"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static WixProjectT SetPreserveTempFiles<WixProjectT>(this WixProjectT project, bool? preserveTempFiles = null, bool noThrow = false, DLL assembly = null) where WixProjectT : WixProject
        {
            project.PreserveTempFiles =
                preserveTempFiles ??
                GetAssemblyAttribute<AssemblyConfigurationAttribute>(noThrow, assembly)?.Configuration?.ToUpper()?.Contains("DEBUG") ??
                project.PreserveTempFiles;
            return project;
        }

        /// <summary>
        /// Sets the project name
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="name">Name shown in installers</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static WixProjectT SetName<WixProjectT>(this WixProjectT project, string name = null, bool noThrow = false, DLL assembly = null) where WixProjectT : WixProject
        {
            project.Name = 
                name ?? 
                GetAssemblyAttribute<AssemblyInsideInstallerNameAttribute>(noThrow, assembly)?.ProductNameFull ??
                project.Name;
            return project;
        }

        /// <summary>
        /// Sets the project output file name
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="outFileName">name excluding extension</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static WixProjectT SetOutFileName<WixProjectT>(this WixProjectT project, string outFileName = null, bool noThrow = false, DLL assembly = null) where WixProjectT : WixProject
        {
            project.OutFileName =
                outFileName ??
                GetAssemblyAttribute<AssemblyProductAttribute>(noThrow, assembly)?.Product ??
                project.OutFileName;
            return project;
        }

        /// <summary>
        /// Sets the project version
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="version">must be valid version like string</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static WixProjectT SetVersion<WixProjectT>(this WixProjectT project, string version = null, bool noThrow = false, DLL assembly = null) where WixProjectT : WixProject
        {
            version = version ?? GetAssemblyAttribute<AssemblyFileVersionAttribute>(noThrow, assembly)?.Version;
            if(version!=null)
            {
                var ver = new Version(version);
                if (project is Project proj)
                    proj.Version = ver;
                else if (project is Bundle bundle)
                    bundle.Version = ver;
            }
            return project;
        }

        /// <summary>
        /// Sets the project manufacturer
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="manufacturer">manufacturer name</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static WixProjectT SetManufacturer<WixProjectT>(this WixProjectT project, string manufacturer = null, bool noThrow = false, DLL assembly = null) where WixProjectT : WixProject
        {
            manufacturer = manufacturer ?? GetAssemblyAttribute<AssemblyCompanyAttribute>(noThrow, assembly)?.Company;

            if (project is Project proj)
                proj.ControlPanelInfo.Manufacturer = manufacturer;
            else if (project is Bundle bundle)
                bundle.Manufacturer = manufacturer;
            return project;
        }

    }
}
