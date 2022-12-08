using System;
using System.Xml.Linq;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Simple object converted to Wix UpgradeVersion XML tag.
    /// 
    /// https://wixtoolset.org/docs/v3/xsd/wix/upgradeversion/
    /// 
    /// </summary>
    public class UpgradeVersion : WixObject, IXmlAware
    {
        /// <summary>
        /// Set to "yes" to detect all languages, excluding the languages listed in the Language attribute.
        /// </summary>
        [Xml]
        public bool? ExcludeLanguages;

        /// <summary>
        /// Set to "yes" to continue installation upon failure to remove a product or application.
        /// </summary>
        [Xml]
        public bool? IgnoreRemoveFailure;

        /// <summary>
        /// Set to "yes" to make the range of versions detected include the value specified in Maximum.
        /// </summary>
        [Xml]
        public bool? IncludeMaximum;

        /// <summary>
        /// Set to "no" to make the range of versions detected exclude the value specified in Minimum. This attribute is "yes" by default.
        /// </summary>
        [Xml]
        public bool? IncludeMinimum;

        /// <summary>
        /// Specifies the set of languages detected by FindRelatedProducts. 
        /// Enter a list of numeric language identifiers (LANGID) separated by commas (,). 
        /// Leave this value null to specify all languages. 
        /// Set ExcludeLanguages to "yes" in order detect all languages, excluding the languages listed in this value.
        /// </summary>
        [Xml]
        public string Language;

        /// <summary>
        /// Specifies the upper boundary of the range of product versions detected by FindRelatedProducts.
        /// </summary>
        [Xml]
        public string Maximum;

        /// <summary>
        /// Set to "yes" to migrate feature states from upgraded products by enabling the logic in the MigrateFeatureStates action.
        /// </summary>
        [Xml]
        public bool? MigrateFeatures;

        /// <summary>
        /// Specifies the lower bound on the range of product versions to be detected by FindRelatedProducts.
        /// </summary>
        [Xml]
        public string Minimum;

        /// <summary>
        /// Set to "yes" to detect products and applications but do not uninstall.
        /// </summary>
        [Xml]
        public bool? OnlyDetect;

        /// <summary>
        /// When the FindRelatedProducts action detects a related product installed on the system, 
        /// it appends the product code to the property specified in this field. 
        /// Windows Installer documentation for the Upgrade table states that the property specified 
        /// in this field must be a public property and must be added to the SecureCustomProperties property. 
        /// WiX automatically appends the property specified in this field to the SecureCustomProperties property when creating an MSI. 
        /// Each UpgradeVersion must have a unique Property value. 
        /// After the FindRelatedProducts action is run, the value of this property is a list of product codes, 
        /// separated by semicolons (;), detected on the system.
        /// </summary>
        [Xml]
        public string Property;

        /// <summary>
        /// The installer sets the REMOVE property to features specified in this column. 
        /// The features to be removed can be determined at run time.
        /// The Formatted string entered in this field must evaluate to a comma-delimited list of feature names. 
        /// For example: [Feature1],[Feature2],[Feature3]. 
        /// No features are removed if the field contains formatted text that evaluates to an empty string.
        /// The installer sets REMOVE=ALL only if the Remove field is empty.
        /// </summary>
        [Xml]
        public string RemoveFeatures;

        /// <summary>
        /// Gets the output xml file.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public XElement ToXml()
        {
            if (string.IsNullOrEmpty(Property))
                throw new ArgumentNullException("Property attribute cannot be empty");
            return this.ToXElement("UpgradeVersion");
        }
    }
}
