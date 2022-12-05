using System.IO;
using System.Linq;
using WixSharp.Fluent.XML;
using WixSharp.CommonTasks;
using System.Reflection;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Path splitting helper
    /// </summary>
    public static class PathExtensions
    {
        static readonly FieldInfo lastDirField = typeof(Dir).GetField("lastDir", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Gets the last dir from a dir, usefull when operating on dirs made with path containing multiple dirs
        /// </summary>
        /// <typeparam name="DirT"></typeparam>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Dir GetLastDir<DirT>(this DirT dir) where DirT : Dir
        {
            return lastDirField.GetValue(dir) as Dir;
        }

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
        /// If applied on a dir made with complex path like "%something%\asd\asd" or "asd\asd", it will be applied only on the root of the complex path
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
        /// Use only after testing if the dir persists for some reason...
        /// If applied on a dir made with complex path like "%something%\asd\asd" or "asd\asd", it will be applied only on the root of the complex path
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
        /// Use only after testing if the dir persists for some reason...
        /// If placed on a dir made with complex path like "%something%\asd\asd" or "asd\asd", it will be applied recursively  
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
        /// Perform an operation on the last created dir (the one most deep), usefull for permissions and other stuff that could be applied on the root dir instead
        /// </summary>
        /// <typeparam name="DirT"></typeparam>
        /// <param name="dir"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static DirT WithLastDir<DirT>(this DirT dir, System.Action<Dir> action) where DirT : Dir
        {
            dir.GetLastDir().With(action);
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

        /// <summary>
        /// Sets The KeyPath Attribute
        /// </summary>
        /// <typeparam name="FileT"></typeparam>
        /// <param name="file"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FileT SetKeyPath<FileT>(this FileT file, bool? value = null) where FileT : File
        {
            file.Attributes.Add("KeyPath", (value??true) ? "yes" : "no");
            return file;
        }
    }
}
