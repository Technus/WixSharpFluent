using System;
using System.Collections.Generic;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Provides a way to assign consistent GUIDs.
    /// </summary>
    public static class GuidDictionary
    {
        private static readonly IDictionary<string, Guid> guidMap = new Dictionary<string, Guid>(StringComparer.InvariantCulture);
        private static readonly List<string> ComponentPrefixes = new List<string>()
        {
            "Component."
        };
        private static readonly List<string> ComponentSuffixes = new List<string>()
        {
            ".EmptyDirectory",
            ".VirtDir"
        };


        static GuidDictionary()
        {
            Initialize();
        }

        /// <summary>
        /// Initialized automatically on load time of this class and <see cref="WixCommonExtensions"/> class
        /// </summary>
        internal static void Initialize()
        {
            WixGuid.Generator = GenerateGuid;
        }

        /// <summary>
        /// Generates or gets from dictionary, the GUID for specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid GenerateGuid(object obj)
        {
            if (obj == null)
                return GuidGenerators.Default(obj);

            var str = obj.ToString();

            foreach (var prefix in ComponentPrefixes)
                if (str.StartsWith(prefix))
                {
                    str = str.Substring(prefix.Length);
                    break;
                }

            foreach (var suffix in ComponentSuffixes)
                if (str.EndsWith(suffix))
                {
                    str = str.Substring(0,str.Length-suffix.Length);
                    break;
                }

            var guid = guidMap.ContainsKey(str) ? guidMap[str] : GuidGenerators.Default(obj);

            //Console.WriteLine($"GEN: {obj} as {str} to {guid}");

            return guid;
        }

        /// <summary>
        /// Assigns guid to object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="guid"></param>
        /// <returns>the object ToString used as key</returns>
        public static string AssignGuid(this object id, Guid guid)
        {
            var str = id.ToString();
            if (str.Contains("."))
                throw new ArgumentException($"The ID can not contain '.': {str}",nameof(id));
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("The string is empty", nameof(id));
            guidMap.Add(str, guid);
            return str;
        }

        /// <summary>
        /// Unassigns guid from object
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the object ToString used as key</returns>
        public static string UnassignGuid(this object id)
        {
            var str = id.ToString();
            guidMap.Remove(str);
            return str;
        }
    }
}
