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
using System.Xml.Serialization;

namespace ISHDeploy.Common.Models.TranslationOrganizer
{
    /// <summary>
    /// <para type="description">Represents configuration of an instance of TMS.</para>
    /// </summary>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("add", Namespace = "", IsNullable = false)]
    public class TmsConfigurationSection : BaseXMLElement
    {
        /// <summary>
        /// The alias of TMS 
        /// </summary>
        [XmlAttribute("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// The Uri to TMS 
        /// </summary>
        [XmlAttribute("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// The userName to get access to TMS
        /// </summary>
        [XmlAttribute("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// The password to get access to TMS
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
        /// The mapping between trisoftLanguage and TMSLocaleId
        /// </summary>
        [XmlArray("mappings")]
        [XmlArrayItem("add", IsNullable = false)]
        public ISHLanguageToTmsLanguageMapping[] Mappings { get; set; }

        /// <summary>
        /// Tms templates
        /// </summary>
        [XmlArray("templates")]
        [XmlArrayItem("add", IsNullable = false)]
        public TmsTemplate[] Templates { get; set; }

        /// <summary>
        /// The requested metadata
        /// </summary>
        [XmlElement("requestedMetadata", Namespace = "")]
        public ISHFieldMetadata RequestedMetadata { get; set; }

        /// <summary>
        /// Returns true if the RequestedMetadata has changed; otherwise, returns false.  
        /// The RequestedMetadata property serializes only if true is returned.
        /// </summary>
        /// <returns></returns>
        public bool RequestedMetadataSpecified => RequestedMetadata != null;

        /// <summary>
        /// The grouping metadata
        /// </summary>
        [XmlElement("groupingMetadata", Namespace = "")]
        public ISHFieldMetadata GroupingMetadata { get; set; }

        /// <summary>
        /// Returns true if the GroupingMetadata has changed; otherwise, returns false.  
        /// The GroupingMetadata property serializes only if true is returned.
        /// </summary>
        /// <returns></returns>
        public bool GroupingMetadataSpecified => GroupingMetadata != null;

        /// <summary>
        /// Prevents a default instance of the <see cref="TmsConfigurationSection"/> class from being created.
        /// </summary>
        private TmsConfigurationSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TmsConfigurationSection"/> class.
        /// </summary>
        /// <param name="alias">The alias of TMS</param>
        /// <param name="uri">The Uri to TMS </param>
        /// <param name="userName">The userName to get access to TMS</param>
        /// <param name="password">The password to get access to TMS</param>
        /// <param name="externalJobMaxTotalUncompressedSizeBytes">The max value of total size in bytes of uncompressed external job</param>
        /// <param name="retriesOnTimeout">The number of retries on timeout</param>
        /// <param name="mappings">The mapping between trisoftLanguage and TmsLanguage</param>
        /// <param name="templates">Tms templates</param>
        /// <param name="requestedMetadata">The port number of proxy server</param>
        /// <param name="groupingMetadata">The port number of proxy server</param>
        public TmsConfigurationSection(string alias, string uri, string userName, string password, int externalJobMaxTotalUncompressedSizeBytes, int retriesOnTimeout, ISHLanguageToTmsLanguageMapping[] mappings, TmsTemplate[] templates, ISHFieldMetadata requestedMetadata = null, ISHFieldMetadata groupingMetadata = null)
        {
            Alias = alias;
            Uri = uri;
            UserName = userName;
            Password = password;
            ExternalJobMaxTotalUncompressedSizeBytes = externalJobMaxTotalUncompressedSizeBytes;
            RetriesOnTimeout = retriesOnTimeout;
            Mappings = mappings;
            Templates = templates;

            RequestedMetadata = requestedMetadata;
            GroupingMetadata = groupingMetadata;

            RelativeFilePath = @"TranslationOrganizer\Bin\TranslationOrganizer.exe.config";
            XPathToParentElement = "configuration/trisoft.infoShare.translationOrganizer/tms/instances";
            NameOfItem = "add";

            XPathFormat = "configuration/trisoft.infoShare.translationOrganizer/tms/instances/add";
            XPath = string.Format(XPathFormat, alias);
        }
    }
}
