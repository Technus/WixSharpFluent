using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using WixSharp.Fluent.Attributes;
using DLL = System.Reflection.Assembly;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Contains all reflection helpers
    /// </summary>
    public static class AssemblyAttributeExtensions
    {
        /// <summary>
        /// List of assemblies to exclude while looking up automatically in stack trace for Assembly attributes
        /// </summary>
        public static readonly List<DLL> AssembliesToIgnoreWhileLookingUp = new List<DLL>();

        static AssemblyAttributeExtensions()
        {
            AssembliesToIgnoreWhileLookingUp.Add(DLL.GetExecutingAssembly());
        }

        /// <summary>
        /// stack trace crawl to find a non blacklisted Assembly in <see cref="AssembliesToIgnoreWhileLookingUp"/> 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DLL GetOtherCallingAssembly()
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

        /// <summary>
        /// Gets An arbitrary Attribute from the assembly
        /// </summary>
        /// <typeparam name="AttributeT"></typeparam>
        /// <param name="noThrow"></param>
        /// <param name="assembly">If not specified will do a stack trace crawl to find a non blacklisted Assembly</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static AttributeT GetAssemblyAttribute<AttributeT>(bool noThrow = false, DLL assembly = null) where AttributeT : Attribute
        {
            assembly = assembly ?? GetOtherCallingAssembly();

            var attribute = assembly.GetCustomAttribute<AttributeT>();

            if (!noThrow && attribute == null)
                throw new ArgumentException($"{assembly.FullName} has no {typeof(AttributeT).FullName}", nameof(assembly));

            return attribute;
        }

        /// <summary>
        /// Gets the Configuration that is being build <see cref="AssemblyConfigurationAttribute.Configuration"/>
        /// </summary>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetConfiguration(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyConfigurationAttribute>(noThrow, assembly)?.Configuration;
        }

        /// <summary>
        /// Gets the start menu path from Assembly attribute
        /// </summary>
        /// <param name="prefix">Should be a wix Var like [...] or a rooted path</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetStartMenuPath(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            return GetStartMenuVar(prefix ?? "%ProgramMenu%", noThrow, assembly);
        }

        /// <summary>
        /// Gets the installation path inside program files from Assembly attribute
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetInstallationPath(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            return GetInstallationVar(prefix ?? "%ProgramFiles%", noThrow, assembly);
        }

        /// <summary>
        /// Gets the path inside App Data from Assembly attribute
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAppDataPath(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            return GetAppDataVar(prefix ?? "%CommonAppData%", noThrow,assembly);
        }

        /// <summary>
        /// Gets the path inside Common files from Assembly attribute
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetCommonFilesPath(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            return GetCommonFilesVar(prefix ?? "%CommonFiles%", noThrow, assembly);
        }

        /// <summary>
        /// Gets the start menu path from Assembly attribute
        /// </summary>
        /// <param name="prefix">Should be a wix Var like [...] or a rooted path</param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetStartMenuVar(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            prefix = prefix ?? "[ProgramMenuFolder]";
            if (!prefix.StartsWith("["))
                prefix += "\\";
            return prefix + GetAssemblyAttribute<AssemblyStartMenuPathAttribute>(noThrow, assembly)?.Path ?? "";
        }

        /// <summary>
        /// Gets the installation path inside program files from Assembly attribute
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetInstallationVar(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            prefix = prefix ?? "[ProgramFilesFolder]";
            if (!prefix.StartsWith("["))
                prefix += "\\";
            return prefix + GetAssemblyAttribute<AssemblyProgramFilesPathAttribute>(noThrow, assembly)?.Path ?? "";
        }

        /// <summary>
        /// Gets the path inside App Data from Assembly attribute
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAppDataVar(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            prefix = prefix ?? "[CommonAppDataFolder]";
            if (!prefix.StartsWith("["))
                prefix += "\\";
            return prefix + GetAssemblyAttribute<AssemblyAppDataPathAttribute>(noThrow, assembly)?.Path ?? "";
        }

        /// <summary>
        /// Gets the path inside Common files from Assembly attribute
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetCommonFilesVar(string prefix = null, bool noThrow = false, DLL assembly = null)
        {
            prefix = prefix ?? "[CommonFilesFolder]";
            if (!prefix.StartsWith("["))
                prefix += "\\";
            return prefix + GetAssemblyAttribute<AssemblyCommonFilesPathAttribute>(noThrow, assembly)?.Path ?? "";
        }

        /// <summary>
        /// Gets the source path of files to include
        /// </summary>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetSourcePath(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblySourcePathAttribute>(noThrow, assembly)?.Path;
        }

        /// <summary>
        /// Gets the executable name inside source path to create shortcuts for 
        /// </summary>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetExecutableName(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyExecutableNameAttribute>(noThrow, assembly)?.ExecutableName;
        }

        /// <summary>
        /// Gets The ConstantDefines from MSBuildScript
        /// </summary>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static AssemblyDefinesAttribute GetDefines(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyDefinesAttribute>(noThrow, assembly);
        }
    }
}
