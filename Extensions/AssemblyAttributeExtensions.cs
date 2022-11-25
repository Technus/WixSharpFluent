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
            AssembliesToIgnoreWhileLookingUp.Add(typeof(WixCommonExtensions).Assembly);
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

        internal static AttributeT GetAssemblyAttribute<AttributeT>(bool noThrow = false, DLL assembly = null) where AttributeT : Attribute
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
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetStartMenuPath(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyStartMenuPathAttribute>(noThrow, assembly)?.Path;
        }

        /// <summary>
        /// Gets the installation path inside program files from Assembly attribute
        /// </summary>
        /// <param name="noThrow"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetInstallationPath(bool noThrow = false, DLL assembly = null)
        {
            return GetAssemblyAttribute<AssemblyProgramFilesPathAttribute>(noThrow, assembly)?.Path;
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
    }
}
