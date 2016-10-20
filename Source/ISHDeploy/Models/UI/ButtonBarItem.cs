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
using System;
using System.Xml;
using System.Xml.Serialization;

namespace ISHDeploy.Models.UI
{
    /// <summary>
    /// <para type="description">Represents collection of ButtonBarItem.</para>
    /// </summary>
    [XmlRoot("BUTTONBAR", Namespace = "")]
    public class ButtonBarItemCollection
    {
        /// <summary>
        /// Array of ButtonBarItems.
        /// </summary>
        [XmlElement("BUTTON")]
        public ButtonBarItem[] ButtonBarItemArray { get; set; }
    }

    /// <summary>
    /// Java script Node
    /// </summary>
    [XmlRoot("SCRIPT", Namespace = "")]
    public class Script
    {
        /// <summary>
        /// LANGUAGE
        /// </summary>
        [XmlAttribute("LANGUAGE")]
        public string Language { get; set; }
        /// <summary>
        /// type
        /// </summary>
        [XmlAttribute("type")]
        public string Type { get; set; }
        /// <summary>
        /// src
        /// </summary>
        [XmlAttribute("src")]
        public string Src { get; set; }

        /// <summary>
        /// Content dor CDData part.
        /// </summary>
        [XmlIgnore]
        public string Content { get; set; }

        /// <summary>
        /// text body
        /// </summary>
        [XmlText]
        public XmlNode[] ScriptBody
        {
            get
            {
                if (Content == null)
                    return null;
                return new XmlNode[] { new XmlDocument().CreateCDataSection(Content) };
            }
            set
            {
                if (value == null)
                {
                    Content = null;
                    return;
                }

                if (value.Length != 1)
                {
                    throw new InvalidOperationException($"Invalid array length {value.Length}");
                }

                var node0 = value[0];
                var cdata = node0 as XmlCDataSection;
                if (cdata == null)
                {
                    throw new InvalidOperationException($"Invalid node type {node0.NodeType}");
                }

                Content = cdata.Data;
            }
        }
    }


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
        /// Javascript file.
        /// </summary>
        [XmlElement("SCRIPT")]
        public Script[] Script { set; get; }

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
        /// Change some fileds for created one.
        /// </summary>
        public void ChangeButtonBarItemProperties(string fileName)
        {
            RelativeFilePath = $@"Author\ASP\XSL\{fileName}";
            NameOfRootElement = "BUTTONBAR";
            NameOfItem = "BUTTON";
            XPathFormat = "BUTTONBAR/BUTTON/INPUT[@NAME='{0}']/parent::BUTTON";
            XPath = string.Format(XPathFormat, Input.Name);
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
        /// <param name="jArgumentList">Hides text.</param>
        public ButtonBarItem(string buttonBar, string name, string[] ishtype = null, string icon = null, string onClick = null, object[] jArgumentList = null, string checkaccess = null, bool hideText = false)
        {
            RelativeFilePath = $@"Author\ASP\XSL\{buttonBar}";
            NameOfRootElement = "BUTTONBAR";
            NameOfItem = "BUTTON";

            Input = new Input();
            Input.Value = name;
            Input.Name = name;
            Input.Icon = icon;
            Input.OnClick = onClick;
            CardTypes = ishtype;

            CheckAccess = checkaccess;
            XPathFormat = "BUTTONBAR/BUTTON/INPUT[@NAME='{0}']/parent::BUTTON";
            XPath = string.Format(XPathFormat, name);

            if (hideText)
            {
                Input.Showtext = "N";
            }

            if (jArgumentList != null)
            {
                // will add ' before and after in a case of string type
                for (int index = 0; index < jArgumentList.Length; index++)
                    if (jArgumentList[index].GetType() == typeof(string))
                        jArgumentList[index] = "'" + jArgumentList[index] + "'";

                string parameters = string.Join(", ", jArgumentList); //'Hello Alex!', true, 0
                Input.OnClick = $@"Trisoft.Helpers.ExtensionsLoader.executeExtension('{onClick}', [{parameters}])";
                Script = new Script[] {
                    new Script{
                        Language = "JAVASCRIPT",
                        Type = "text/javascript",
                        Src="../../UI/Helpers/ExtensionsLoader.js"
                        },
                    new Script{
                        Language = "JAVASCRIPT",
                        Content = @"
      // Load the extension resources
      Trisoft.Helpers.ExtensionsLoader.enableExtensions(""../../"");"
                        }

                };
            }
        }
    }
}
