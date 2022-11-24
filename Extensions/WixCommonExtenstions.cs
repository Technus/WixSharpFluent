using System;
using WixSharp.Bootstrapper;
using System.Diagnostics;
using System.Reflection;
using DLL = System.Reflection.Assembly;
using WixSharp.Fluent.Attributes;
using System.Collections.Generic;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Extensions for All types of Wix Projects
    /// </summary>
    public static class WixCommonExtensions
    {
        internal static string wixOutputFolder = "wix";

        public static readonly List<DLL> AssembliesToIgnoreWhileLookingUp = new List<DLL>();

        static WixCommonExtensions()
        {
            GuidDictionary.Initialize();
            Compiler.AllowNonRtfLicense = true;//Fine since only Hyperlink is allowed anyway
            MSBuild.EmitAutoGenFiles = false;

            AssembliesToIgnoreWhileLookingUp.Add(typeof(WixCommonExtensions).Assembly);
        }

        /// <summary>
        /// Sets Manufacturer, Includes Extensions and makes the Temp Files preserved for debugging purposes.
        /// </summary>
        /// <typeparam name="WixProjectT"></typeparam>
        /// <param name="project"></param>
        /// <param name="preserveTempFiles"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static WixProjectT SetWixDefaults<WixProjectT>(this WixProjectT project, bool noThrow = false, DLL assembly = null) where WixProjectT : WixProject
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

        public static WixProjectT AddExtensions<WixProjectT>(this WixProjectT project,params WixExtension[] extensions) where WixProjectT : WixProject
        {
            foreach (var extension in extensions)
            {
                project.Include(extension);
            }
            return project;
        }

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

        private static DLL GetOtherCallingAssembly()
        {
            var frames = new StackTrace().GetFrames();
            for (int frameNumber = 1; frameNumber < frames.Length; frameNumber++)
            {
                DLL other = frames[frameNumber].GetMethod().ReflectedType.Assembly;
                if (!AssembliesToIgnoreWhileLookingUp.Contains(other))
                {
                    return other;
                }
            }

            throw new Exception("Could not identify calling assembly");
        }

        internal static AttributeT GetAssemblyAttribute<AttributeT>(bool noThrow=false, DLL assembly=null) where AttributeT : Attribute
        {
            assembly = assembly ?? GetOtherCallingAssembly();

            var attribute = assembly.GetCustomAttribute<AttributeT>();

            if (!noThrow && attribute == null)
                throw new ArgumentException($"{assembly.FullName} has no {typeof(AttributeT).FullName}", nameof(assembly));

            return attribute;
        }

        public static AssemblyDefinesAttribute GetDefines(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyDefinesAttribute>(noThrow, assembly);
        }

        /// <summary>
        /// Gets the Configuration that is being build <see cref="AssemblyConfigurationAttribute.Configuration"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetConfiguration(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyConfigurationAttribute>(noThrow, assembly)?.Configuration;
        }

        public static string GetStartMenuPath(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyStartMenuPathAttribute>(noThrow, assembly)?.Path;
        }

        public static string GetInstallationPath(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyProgramFilesPathAttribute>(noThrow, assembly)?.Path;
        }

        public static string GetSourcePath(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblySourcePathAttribute>(noThrow, assembly)?.Path;
        }

        public static string GetExecutableName(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyExecutableNameAttribute>(noThrow, assembly)?.ExecutableName;
        }
    }
}
