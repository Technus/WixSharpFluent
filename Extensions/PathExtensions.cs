using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Path splitting helper
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Replaces all '/' with '\'.
        /// Splits by delimiter.
        /// And removes blank or empty pieces (removes duplicate slashes)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] PathSplitCleanup(this string path,char delimiter)
        {
            return path.Replace('/', '\\')
                       .Split(delimiter)
                       .Where(s => !string.IsNullOrWhiteSpace(s))
                       .Select(s => s.Trim())
                       .ToArray();
        }

        /// <summary>
        /// Separates single path to its components
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] PathToPathPieces(this string path)
        {
            return path.PathSplitCleanup('\\').ToArray();
        }

        /// <summary>
        /// Separates multiple paths to their components
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string[][] PathsToPathPieces(this string paths)
        {
            return paths.PathSplitCleanup(';').Select(path => path.PathToPathPieces()).ToArray();
        }

        /// <summary>
        /// Separates multiple paths to an array of paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string[] PathsToPathArray(this string paths)
        {
            return paths.PathsToPathPieces().Select(path=>path.JoinBy("\\")).ToArray();
        }

        /// <summary>
        /// Sets the Permission for everyone on the Dir
        /// </summary>
        /// <typeparam name="DirT"></typeparam>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirT SetPermisssionEveryone<DirT>(this DirT dir) where DirT : Dir
        {
            dir.Permissions = dir.Permissions.Combine(new DirPermission()
            {
                User = "Everyone",
                GenericAll = true,
            });
            return dir;
        }

        /// <summary>
        /// Adds the file to firewall exceptions
        /// </summary>
        /// <typeparam name="FileT"></typeparam>
        /// <param name="file"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FileT SetFireWallException<FileT>(this FileT file, string name = null) where FileT : File
        {
            name = name ?? System.IO.Path.GetFileName(file.TargetFileName ?? file.Name);
            file.FirewallExceptions = file.FirewallExceptions.Combine(new FirewallException()
            {
                Scope = FirewallExceptionScope.any,
                IgnoreFailure = true,
                Profile = FirewallExceptionProfile.all,
                Name = name,
            });
            return file;
        }
    }
}
