using System;
using System.Collections.Generic;
using System.Linq;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Attribute to store Defined Constants
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyDefinesAttribute : Attribute
    {
        /// <summary>
        /// The MSBuild DefineConstants variable
        /// </summary>
        public string DefineConstants { get; set; }

        /// <summary>
        /// The split MSBuild DefineConstants variable
        /// </summary>
        public string[] DefineList
        {
            get
            {
                return DefineConstants.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
            }
        }

        /// <summary>
        /// Dictionary containing all defines as key value pairs
        /// </summary>
        public Dictionary<string, string> Defines
        {
            get
            {
                var dict = new Dictionary<string, string>();
                foreach (var define in DefineList)
                {
                    var split = define.Split('=').Select(s => s.Trim()).ToArray();
                    if (split.Length == 2)
                        dict.Add(split[0], split[1]);
                    else dict.Add(split[0], "");
                }
                return dict;
            }
        }
    }
}
