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

namespace ISHDeploy.Models.UI
{
    /// <summary>
    /// <para type="description">Base class to represent UI element</para>
    /// </summary>
    [XmlInclude(typeof(MainMenuBarItem))]
    public abstract class BaseUIElement
    {
        /// <summary>
        /// Gets or sets the relative file path.
        /// </summary>
        /// <value>
        /// The relative file path.
        /// </value>
        [XmlIgnore]
        public string RelativeFilePath { get; protected set; }

        /// <summary>
        /// Gets or sets the name of root element.
        /// </summary>
        /// <value>
        /// The root path.
        /// </value>
        [XmlIgnore]
        public string NameOfRootElement { get; protected set; }

        /// <summary>
        /// Gets or sets the name of item.
        /// </summary>
        /// <value>
        /// The child item path.
        /// </value>
        [XmlIgnore]
        public string NameOfItem { get; protected set; }

        /// <summary>
        /// Gets or sets the key attribute.
        /// </summary>
        /// <value>
        /// The key attribute.
        /// </value>
        [XmlIgnore]
        public string KeyAttribute { get; protected set; }
    }
}
