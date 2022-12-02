using System;
using System.Xml.Linq;

namespace WixSharp.Fluent.XML
{
    internal class CreateFolder : WixEntity, IGenericEntity
    {
        public void Process(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util);

            XElement element = this.ToXElement("CreateFolder");

            context.XParent
                   .AddElement("Component")
                   .SetAttribute("Id", ComponentId ?? $"{Id ?? context.XParent.GetAttribute("Id")}.Component")
                   .SetAttribute("Guid", Guid.NewGuid())
                   .Add(element);
        }
    }
}
