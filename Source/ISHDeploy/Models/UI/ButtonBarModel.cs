/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using ISHDeploy.Business.Enums;
using System.Xml.Serialization;

namespace ISHDeploy.Models.UI
{
    [XmlRoot("BUTTON", Namespace = "")]
    public class ButtonBarModel : BaseUIModel
    {
        [XmlElement("CARDTYPE")]
        public CardType[] ISHTYPE { set; get; }

        [XmlElement("INPUT")]
        public Input Input { set; get; }

        private ButtonBarModel()
        {

        }

        public ButtonBarModel(ButtonBarType buttonBar, string name, CardType[] ishtype = null, string icon = null, string onClick = null, string checkaccess = null)
        {
            RelativeFilePath = $@"Author\ASP\XSL\{buttonBar}.xml";
            RootPath = "BUTTONBAR";
            ChildItemPath = "BUTTON";
            KeyAttribute = "NAME";

            Input = new Input();
            Input.Value = name;
            Input.Name = name;
            Input.Icon = icon;
            Input.OnClick = onClick;
            ISHTYPE= ishtype;

        }
    }
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
