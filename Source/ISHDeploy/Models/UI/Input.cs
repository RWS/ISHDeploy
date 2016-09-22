using System.Xml.Serialization;

namespace ISHDeploy.Models.UI
{
    public class Input
    {
        [XmlAttribute("type")]
        public string Type { set; get; } = "button";

        [XmlAttribute("NAME")]
        public string Name { set; get; }

        [XmlAttribute("onClick")]
        public string OnClick { set; get; }

        [XmlAttribute("VALUE")]
        public string Value { set; get; }

        [XmlAttribute("CLASS")]
        public string Class { set; get; } = "button";

        [XmlAttribute("ICON")]
        public string Icon { set; get; }
    }
}
