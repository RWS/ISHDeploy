using System.Xml.Serialization;

namespace ISHDeploy.Models.UI.CUIFConfig
{
    /// <summary>
    /// <para type="description">Internal class to represent a ResourceGroup element of Catalina _config.xml file.</para>
    /// </summary>
    [XmlRoot("resourceGroup", Namespace = "")]
    public class ResourceGroup : BaseUIElement
    {
        /// <summary>
        /// Gets the name of item.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { set; get; }


        /// <summary>
        /// Gets array of files of the resource group.
        /// </summary>
        [XmlArray("files")]
        public File[] Files { set; get; }

        /// <summary>
        /// Prevents a default instance of the <see cref="ResourceGroup"/> class from being created.
        /// </summary>
        private ResourceGroup()
        {
            RelativeFilePath = @"Author\ASP\UI\Extensions\_config.xml";
            XPathToParentElement = "configuration/resourceGroups";
            NameOfItem = "resourceGroup";

            XPathFormat = "configuration/resourceGroups/resourceGroup[@name='{0}']";
            XPath = string.Format(XPathFormat, Name);
        }
    }
}
