using System.Xml.Serialization;

namespace ISHDeploy.Models.UI
{    
     /// <summary>
     /// <para type="description">Internal clalss for ButtonBar usage.</para>
     /// </summary>
    public class Input
    {
        /// <summary>
        /// Gets the name of item.
        /// </summary>
        [XmlAttribute("type")]
        public string Type { set; get; } = "button";
        
        /// <summary>
        /// Gets the name of item.
        /// </summary>
        [XmlAttribute("NAME")]
        public string Name { set; get; }

        /// <summary>
        /// Javascript to be executed after click.
        /// </summary>
        [XmlAttribute("onClick")]
        public string OnClick { set; get; }
        
        /// <summary>
        /// Message to display.
        /// </summary>
        [XmlAttribute("VALUE")]
        public string Value { set; get; }

        /// <summary>
        /// Define a class - button.
        /// </summary>
        [XmlAttribute("CLASS")]
        public string Class { set; get; } = "button";

        /// <summary>
        /// Correspond icon.
        /// </summary>
        [XmlAttribute("ICON")]
        public string Icon { set; get; }
        
        
        /// <summary>
        /// Correspond icon.
        /// </summary>
        [XmlAttribute("SHOWTEXT")]
        public string Showtext { set; get; }
    }
}
