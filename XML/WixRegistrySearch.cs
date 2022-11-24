using WixSharp.Bootstrapper;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Extends the <see cref="UtilRegistrySearch"/> by adding the Id as field Instead of using custom attributes./>
    /// </summary>
    public class WixRegistrySearch : UtilRegistrySearch
    {
        /// <summary>
        /// The Wix XML tag Id attribute. (Was missing; Readded) 
        /// </summary>
        [Xml]
        public string Id;
    }
}
