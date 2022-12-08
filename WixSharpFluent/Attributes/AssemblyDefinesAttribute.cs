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
        public Dictionary<string, List<string>> Defines
        {
            get
            {
                var dict = new Dictionary<string, List<string>>();
                foreach (var define in DefineList)
                {
                    var split = define.Split(new char[] { '=' },2).Select(s => s.Trim()).ToArray();
                    if(split.Length > 1)
                    {
                        if (dict.ContainsKey(split[0]))
                            dict[split[0]].Add(split[1]);
                        else
                            dict[split[0]] = new List<string>() { split[1] };
                    }
                    else
                    {
                        if (dict.ContainsKey(split[0]))
                            dict[split[0]].Add("");
                        else
                            dict[split[0]]=new List<string>() { "" };
                    }
                }
                return dict;
            }
        }
    }
}
