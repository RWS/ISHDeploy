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

using System.Xml.Linq;
using System.Xml.Serialization;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    /// <para type="description">Base class to represent XML element</para>
    /// </summary>
    public abstract class BaseXMLElement
    {
        /// <summary>
        /// Gets the relative file path.
        /// </summary>
        /// <value>
        /// The relative file path.
        /// </value>
        [XmlIgnore]
        public string RelativeFilePath { get; protected set; }

        /// <summary>
        /// Gets the XPath to parent element.
        /// </summary>
        /// <value>
        /// The root path.
        /// </value>
        [XmlIgnore]
        public string XPathToParentElement { get; protected set; }

        /// <summary>
        /// Gets the name of item.
        /// </summary>
        /// <value>
        /// The child item path.
        /// </value>
        [XmlIgnore]
        public string NameOfItem { get; protected set; }

        /// <summary>
        /// Gets the XPath that allows to definitely find this element in XML document.
        /// </summary>
        /// <value>
        /// The root path.
        /// </value>
        [XmlIgnore]
        public string XPath { get; protected set; }

        /// <summary>
        /// Gets the format of XPath that allows to find other element in XML document.
        /// </summary>
        /// <value>
        /// The root path.
        /// </value>
        [XmlIgnore]
        public string XPathFormat { get; protected set; }

        /// <summary>
        /// Gets the format of XPath to define special node to create new element before this special element in XML document.
        /// </summary>
        /// <value>
        /// The root path.
        /// </value>
        [XmlIgnore]
        public string InsertBeforeSpecialXPath { get; protected set; }

        /// <summary>
        /// If node need comment.
        /// </summary>
        /// <value>
        /// The root path.
        /// </value>
        [XmlIgnore]
        public XComment CommentNode { get; protected set; }
    }
}
