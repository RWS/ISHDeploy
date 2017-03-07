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
    /// <para type="description">Represents configuration of an instance of WorldServer.</para>
    /// </summary>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("add", Namespace = "", IsNullable = false)]
    public class WorldServerConfigurationSection : BaseXMLElement
    {
        /// <summary>
        /// The alias of WorldServer 
        /// </summary>
        [XmlAttribute("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// The Uri to WorldServer 
        /// </summary>
        [XmlAttribute("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// The userName to get access to WorldServer
        /// </summary>
        [XmlAttribute("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// The password to get access to WorldServer
        /// </summary>
        [XmlAttribute("password")]
        public string Password { get; set; }

        /// <summary>
        /// The max value of total size in bytes of uncompressed external job
        /// </summary>
        [XmlAttribute("externalJobMaxTotalUncompressedSizeBytes")]
        public int ExternalJobMaxTotalUncompressedSizeBytes { get; set; }

        /// <summary>
        /// The number of retries on timeout
        /// </summary>
        [XmlAttribute("retriesOnTimeout")]
        public int RetriesOnTimeout { get; set; }

        /// <summary>
        /// The number of retries on timeout
        /// </summary>
        [XmlAttribute("apiProtocol")]
        public string ApiProtocol { get; set; }

        /// <summary>
        /// The mapping between trisoftLanguage and worldServerLocaleId
        /// </summary>
        [XmlArray("mappings")]
        [XmlArrayItem("add", IsNullable = false)]
        public ISHLanguageToWorldServerLocaleIdMapping[] Mappings { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="WorldServerConfigurationSection"/> class from being created.
        /// </summary>
        private WorldServerConfigurationSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldServerConfigurationSection"/> class.
        /// </summary>
        /// <param name="alias">The alias of WorldServer</param>
        /// <param name="uri">The Uri to WorldServer </param>
        /// <param name="userName">The userName to get access to WorldServer</param>
        /// <param name="password">The password to get access to WorldServer</param>
        /// <param name="externalJobMaxTotalUncompressedSizeBytes">The max value of total size in bytes of uncompressed external job</param>
        /// <param name="retriesOnTimeout">The number of retries on timeout</param>
        /// <param name="apiProtocol">The type of the api: SOAP or REST</param>
        /// <param name="mappings">The mapping between trisoftLanguage and worldServerLocaleId</param>
        public WorldServerConfigurationSection(string alias, string uri, string userName, string password, int externalJobMaxTotalUncompressedSizeBytes, int retriesOnTimeout, string apiProtocol, ISHLanguageToWorldServerLocaleIdMapping[] mappings)
        {
            Alias = alias;
            Uri = uri;
            UserName = userName;
            Password = password;
            ExternalJobMaxTotalUncompressedSizeBytes = externalJobMaxTotalUncompressedSizeBytes;
            RetriesOnTimeout = retriesOnTimeout;
            ApiProtocol = apiProtocol;
            Mappings = mappings;

            RelativeFilePath = @"TranslationOrganizer\Bin\TranslationOrganizer.exe.config";
            XPathToParentElement = "configuration/trisoft.infoShare.translationOrganizer/worldServer/instances";
            NameOfItem = "add";

            XPathFormat = "configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add[@alias='{0}']";
            XPath = string.Format(XPathFormat, alias);
        }
    }
}
