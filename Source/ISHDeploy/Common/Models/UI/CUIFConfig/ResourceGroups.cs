using System;
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

namespace ISHDeploy.Common.Models.UI.CUIFConfig
{
    /// <summary>
    /// <para type="description">Represents collection of resourceGroup.</para>
    /// </summary>
    [XmlRoot("resourceGroups", Namespace = "")]
    public class ResourceGroups
    {
        /// <summary>
        /// Array of resourceGroups.
        /// </summary>
        [XmlElement("resourceGroup")]
        public ResourceGroup[] resources { get; set; }
    }
}
