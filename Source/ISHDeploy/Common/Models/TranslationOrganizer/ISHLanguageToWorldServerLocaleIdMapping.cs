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

namespace ISHDeploy.Common.Models.TranslationOrganizer
{
    /// <summary>
    /// <para type="description">Represents mapping between trisoftLanguage and worldServerLocaleId.</para>
    /// </summary>
    [System.Serializable()]
    [XmlRoot("add", Namespace = "")]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ISHLanguageToWorldServerLocaleIdMapping
    {
        /// <summary>
        /// The trisoft language ID
        /// </summary>
        [XmlAttribute("trisoftLanguage")]
        public string ISHLanguage { get; set; }

        /// <summary>
        /// The WorldServer language ID
        /// </summary>
        [XmlAttribute("worldServerLocaleId")]
        public int WSLocaleID { get; set; }
    }
}
