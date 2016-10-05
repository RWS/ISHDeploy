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
using System.Linq;

namespace ISHDeploy.Models.UI
{
    /// <summary>
    /// <para type="description">Represents the item depend on button bar type.</para>
    /// </summary>
    [XmlRoot("BUTTON", Namespace = "")]
    public class ButtonBarItem : BaseUIElement
    {
        /// <summary>
        /// Y or N to check access.
        /// </summary>
        [XmlAttribute("CHECKACCESS")]
        public string CheckAccess { set; get; }

        /// <summary>
        /// List of associated cards.
        /// </summary>
        [XmlElement("CARDTYPE")]
        public string[] CardTypes { set; get; }

        /// <summary>
        /// To create Input type xml node
        /// </summary>
        [XmlElement("INPUT")]
        public Input Input { set; get; }

        /// <summary>
        /// Prevents a default instance of the <see cref="ButtonBarItem"/> class from being created.
        /// </summary>
        private ButtonBarItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonBarItem"/> class.
        /// </summary>
        /// <param name="buttonBar">Button bar type of the item.</param>
        /// <param name="name">Button bar name.</param>
        /// <param name="ishtype">List of associated cards.</param>
        /// <param name="icon">Icon for button bar.</param>
        /// <param name="onClick">Javascript to be executed after click.</param>
        /// <param name="checkaccess">Y or N to check access.</param>
        /// <param name="hideText">Hides text.</param>
        public ButtonBarItem(string buttonBar, string name, string[] ishtype = null, string icon = null, string onClick = null, string checkaccess = null, bool hideText = false)
        {
            RelativeFilePath = $@"Author\ASP\XSL\{buttonBar}";
            NameOfRootElement = "BUTTONBAR";
            NameOfItem = "BUTTON";

            Input = new Input();
            Input.Value = name;
            Input.Name = name;
            Input.Icon = icon;
            Input.OnClick = onClick;

            CheckAccess = checkaccess;
            XPathFormat = "BUTTONBAR/BUTTON/INPUT[@NAME='{0}']/parent::BUTTON";
            XPath = string.Format(XPathFormat, name);

            if (hideText)
            {
                Input.Showtext = "N";
            }
        }
    }
}
