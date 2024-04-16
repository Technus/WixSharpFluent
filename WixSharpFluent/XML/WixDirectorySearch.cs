using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WixSharp.Bootstrapper;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Describes a directory search.
    /// </summary>
    public class WixDirectorySearch : WixObject, IXmlAware
    {
        /// <summary>
        /// Id of the search for ordering and dependency.
        /// </summary>
        [Xml]
        public string Id;

        /// <summary>
        /// Id of the search that this one should come after.
        /// </summary>
        [Xml]
        public string After;

        /// <summary>
        /// Condition for evaluating the search. If this evaluates to false, the search is not executed at all.
        /// </summary>
        [Xml]
        public string Condition;

        /// <summary>
        /// Name of the variable in which to place the result of the search.
        /// </summary>
        [Xml]
        public string Variable;

        /// <summary>
        /// Rather than saving the matching directory path into the variable, 
        /// a DirectorySearch can save an attribute of the matching directory instead.
        /// This attribute's value must be one of the following:
        /// exists - Saves true if a matching directory is found; false otherwise.
        /// </summary>
        [Xml]
        public SearchResult? Result;

        /// <summary>
        /// Directory path to search for.
        /// </summary>
        [Xml]
        public string Path;

        /// <summary>
        /// When set to "yes" and the running bundle is 32-bit, 
        /// Wow64DisableWow64FsRedirection is called before starting the search.
        /// </summary>
        public bool? DisableFileRedirection;

        /// <summary>
        /// Emits WiX XML.
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
        {
            XElement xElement = this.ToXElement(WixExtension.Util.ToXName("DirectorySearch"));
            if(DisableFileRedirection is bool disable)
                xElement.SetAttribute("DisableFileRedirection", disable ? "yes" : "no");
            return xElement;
        }
    }
}
