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
using System.Xml.Serialization;

namespace ISHDeploy.Common.Models.UI
{
    /// <summary>
    ///	<para type="description">Represents the item of MainMenuBar (~\Author\ASP\XSL\MainMenuBar.xml).</para>
    /// </summary>
    /// <seealso cref="BaseUIElement" />
    [XmlRoot("menuitem", Namespace = "")]
    public class MainMenuBarItem : BaseUIElement
    {
        /// <summary>
        /// Gets or sets the label of the menu item.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        [XmlAttribute("label")]
        public string Label { set; get; }

        /// <summary>
        /// Gets or sets users for whom the menu item is available.
        /// </summary>
        /// <value>
        /// The user roles.
        /// </value>
        [XmlElement("userrole")]
        public string[] UserRoles { set; get; }

        /// <summary>
        /// Gets or sets the action which occurs on click on the menu item.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        [XmlAttribute("action")]
        public string Action { set; get; }

        /// <summary>
        /// Gets or sets the menu item identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttribute("id")]
        public string Id { set; get; }

        /// <summary>
        /// Prevents a default instance of the <see cref="MainMenuBarItem"/> class from being created.
        /// </summary>
        private MainMenuBarItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuBarItem"/> class.
        /// </summary>
        /// <param name="label">The label of the menu item.</param>
        /// <param name="userRoles">The users for whom the menu item is available.</param>
        /// <param name="action">The action which occurs on click on the menu item.</param>
        /// <param name="id">The menu item identifier.</param>
        public MainMenuBarItem(string label, string[] userRoles = null, string action = null, string id = null)
        {
            RelativeFilePath = @"Author\ASP\XSL\MainMenuBar.xml";
            XPathToParentElement = "mainmenubar";
            NameOfItem = "menuitem";

            Label = label;
            UserRoles = userRoles;
            Action = action;
            Id = id;
            XPathFormat = "mainmenubar/menuitem[@label='{0}']";
            XPath = string.Format(XPathFormat, Label);
        }
    }
}
