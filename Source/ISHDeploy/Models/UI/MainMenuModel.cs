using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISHDeploy.Models.UI
{
    class MainMenuModel : UIBase
    {
        [UIBindingAttribute ("label")]
        public string _label { set; get; }
        [UIBindingAttribute("userrole", true)]
        public string[] _userRole { set; get; }
        [UIBindingAttribute("action")]
        public string _action { set; get; }
        [UIBindingAttribute("id")]
        public string _id { set; get; }
        public MainMenuModel(string label, string[] userRole, string action, string id)
        {
            element = new XElement("menuitem");
            filePath = @"Author\ASP\XSL\MainMenuBar.xml";
            rootPath = "mainmenubar";
            childItemPath = "menuitem";
            keyAttribute = "label";

            _label = label;
            _userRole = userRole;
            _action = action;
            _id = id;
        }
    }
}
