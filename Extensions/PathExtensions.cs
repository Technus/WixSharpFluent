using System.IO;
using System.Linq;
using WixSharp.Fluent.XML;
using WixSharp.CommonTasks;

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
        /// Creates Dir Permission allowing every one inside
        /// </summary>
        /// <returns></returns>
        public static DirPermission GetPermissionForEveryone()
        {
            return new DirPermission()
            {
                User = "Everyone",
                GenericAll = true,
            };
        }

        /// <summary>
        /// Sets the Permission for everyone on the Dir
        /// </summary>
        /// <typeparam name="DirT"></typeparam>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirT SetPermisssionEveryone<DirT>(this DirT dir) where DirT : Dir
        {
            dir.Permissions = dir.Permissions.Combine(GetPermissionForEveryone());
            return dir;
        }

        /// <summary>
        /// Gets create folder to use in <see cref="Dir"/>
        /// WixSharp auto injectes the matching RemoveFolder, so the body of this method only creates the create folder xml tag
        /// </summary>
        /// <returns></returns>
        public static CreateFolder GetCreateRemoveFolder()
        {
            return new CreateFolder();//
        }

        /// <summary>
        /// Adds remove and create folder to the <see cref="Dir"/>
        /// WixSharp auto injectes the matching RemoveFolder, so the body of internally called method only creates the create folder xml tag
        /// </summary>
        /// <typeparam name="DirT"></typeparam>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static DirT AddCreateRemove<DirT>(this DirT dir) where DirT : Dir
        {
            dir.Add(GetCreateRemoveFolder());
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
            name = name ?? Path.GetFileNameWithoutExtension(file.TargetFileName ?? file.Name);
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
