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


    }
}
