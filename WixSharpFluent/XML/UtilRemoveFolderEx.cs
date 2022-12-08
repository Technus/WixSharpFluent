using System;
using System.Xml.Linq;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Install event for example: used to define on which event to perform action
    /// </summary>
    public enum InstallEvent
    {
        /// <summary>
        /// On Installation
        /// </summary>
        install,
        /// <summary>
        /// On Uninstall
        /// </summary>
        uninstall,
        /// <summary>
        /// On Installation and Uninstall
        /// </summary>
        both
    }

    /// <summary>
    /// util:RemoveFolderEx
    /// </summary>
    public class UtilRemoveFolderEx : WixEntity, IGenericEntity
    {
        /// <summary>
        /// When to perform action
        /// </summary>
        [Xml]
        public InstallEvent? On;

        /// <summary>
        /// Property containing the path
        /// </summary>
        [Xml]
        public string Property;

        /// <summary>
        /// Property containing the Directory
        /// </summary>
        public string Directory;

        /// <summary>
        /// The feature Id to use
        /// </summary>
        public string FeatureId;

        /// <summary>
        /// Does actual processing using <see cref="PseudoProcessWithNewComponent(ProcessingContext)"/>
        /// </summary>
        /// <param name="context"></param>
        public void Process(ProcessingContext context) => PseudoProcessWithNewComponent(context);

#if DEBUG
        /// <summary>
        /// The method demonstrates the correct way of integrating RemoveFolderEx.
        /// <para>
        /// The sample also shows various XML manipulation techniques available with Fluent XElement extensions:
        /// <para>- Auto XML serialization of CLR object with serializable members marked with XMLAttribute.</para>
        /// <para>- XML namespace-transparent lookup method FindSingle.</para>
        /// </para>
        /// </summary>
        /// <param name="context"></param>
        private void PseudoProcessInjectToFirst(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util); //indicate that candle needs to use WixUtilExtension.dll
        
            XElement element = this.ToXElement(WixExtension.Util.ToXName("RemoveFolderEx"));
        
            context.XParent
                   .FindFirst("Component")
                   .Add(element);
        }
#endif

        /// <summary>
        /// This method is for demo purposes only. It show an alternative implementation of the
        /// Process(ProcessingContext) with placing the new element inside of the component.
        /// <para>
        /// The sample also shows various XML manipulation techniques available with Fluent XElement extensions:
        /// <para>- AddElement method. Returns new added element.</para>
        /// <para>- SetAttribute method. Returns the element object, which the attribute has been set to.</para>
        /// </para>
        /// </summary>
        /// <param name="context"></param>
        private void PseudoProcessWithNewComponent(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util);

            XElement element = this.ToXElement(WixExtension.Util.ToXName("RemoveFolderEx"));

            context.XParent
                   .AddElement("Component")
                   .SetAttribute("Id", ComponentId ?? $"{Id}.Component")
                   .SetAttribute("Guid", Guid.NewGuid())
                   .SetAttribute("Directory",Directory)
                   .SetAttribute("Feature", FeatureId)
                   .Add(element);
        }

#if DEBUG
        /// <summary>
        /// This method is for demo purposes only. It shows an alternative implementation of the
        /// Process(ProcessingContext) with placing the new element inside of the component and
        /// associates the component with the new feature 'Test Feature'.
        /// <para>
        /// The sample also shows various XML manipulation techniques available with Fluent XElement extensions:
        /// <para>- Lookup for parent with specified name.</para>
        /// <para>- Passing attribute definition string instead of creating attributes manually.</para>
        /// </para>
        /// </summary>
        /// <param name="context"></param>
        private void PseudoProcessWithNewComponentAndFeature(ProcessingContext context)
        {
            context.Project.Include(WixExtension.Util);
        
            XElement element = this.ToXElement(WixExtension.Util.ToXName("RemoveFolderEx"));
        
            context.XParent
                   .AddElement("Component", "Id=TestComponent;Guid=" + Guid.NewGuid())
                   .Add(element);
        
            context.XParent
                   .Parent("Product")
                   .AddElement("Feature", "Id=TestFeature;Title=Test Feature;Absent=allow;Level=1")
                   .AddElement("ComponentRef", "Id=TestComponent");
        }
#endif
    }
}
