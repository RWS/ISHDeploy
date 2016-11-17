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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace ISHDeploy.Models.UI.CUIFConfig
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRoot("resourceGroup", Namespace = "")]
    public class ResourceGroup : BaseUIElement
    {

        /// <summary>
        /// Change some fileds for created one.
        /// </summary>
        public void ChangeItemProperties(string relativePath)
        {
            RelativeFilePath = relativePath;
            XPathToParentElement = "configuration/resourceGroups";
            NameOfItem = "resourceGroup";
            XPathFormat = "configuration/resourceGroups/resourceGroup[@name='{0}']";
            XPath = string.Format(XPathFormat, name);
        }

        private List<ResourceGroupsResourceGroupFile> filesField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("file", IsNullable = false)]
        public List<ResourceGroupsResourceGroupFile> files
        {
            get { return this.filesField; }
            set { this.filesField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }
    }
}


