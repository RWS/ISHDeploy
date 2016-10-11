using System.Xml.Serialization;

namespace ISHDeploy.Models.UI.CUIFConfig
{
    /// <summary>
    /// <para type="description">Internal class to represent a File element of ResourceGroup of Catalina _config.xml file.</para>
    /// </summary>
    [XmlRoot("file", Namespace = "")]
    public class File
    {
        /// <summary>
        /// Gets the name of item.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { set; get; }
    }
}
