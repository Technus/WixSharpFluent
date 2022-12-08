using System;
using System.Xml.Linq;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Used to inject Create Folder (and automagically Remove Folder) XML tags to a directory
    /// Use only after testing if the dir persists for some reason...
    /// </summary>
    public class CreateFolder : WixEntity, IGenericEntity
    {
        /// <summary>
        /// The XML processsor adding the tag(s)
        /// </summary>
        /// <param name="context"></param>
        public void Process(ProcessingContext context)
        {
            XElement element = this.ToXElement("CreateFolder");

            context.XParent
                   .AddElement("Component")
                   .SetAttribute("Id", ComponentId ?? $"{Id ?? context.XParent.GetAttribute("Id")}.Component")
                   .SetAttribute("Guid", Guid.NewGuid())
                   .Add(element);
        }
    }
}
