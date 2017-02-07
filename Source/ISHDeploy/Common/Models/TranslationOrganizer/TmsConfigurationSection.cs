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
    /// <para type="description">Represents configuration of an instance of TMS.</para>
    /// </summary>
    [System.Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("add", Namespace = "", IsNullable = false)]
    public class TmsConfigurationSection : BaseXMLElement
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
        /// The mapping between trisoftLanguage and worldServerLocaleId
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

        ///// <summary>
        ///// The destination port number
        ///// </summary>
        //[XmlAttribute("destinationPortNumber")]
        //public int? DestinationPortNumber { get; set; }

        ///// <summary>
        ///// The location of Isapi filter
        ///// </summary>
        //[XmlAttribute("isapiFilterLocation")]
        //public string IsapiFilterLocation { get; set; }

        ///// <summary>
        ///// Use compression
        ///// </summary>
        //[XmlAttribute("useCompression")]
        //public bool? UseCompression { get; set; }

        ///// <summary>
        ///// Use SSL
        ///// </summary>
        //[XmlAttribute("useSsl")]
        //public bool? UseSsl { get; set; }

        ///// <summary>
        ///// Use default proxy credentials
        ///// </summary>
        //[XmlAttribute("useDefaultProxyCredentials")]
        //public bool? UseDefaultProxyCredentials { get; set; }

        ///// <summary>
        ///// The proxy server
        ///// </summary>
        //[XmlAttribute("proxyServer")]
        //public string ProxyServer { get; set; }

        ///// <summary>
        ///// The port number of proxy server
        ///// </summary>
        //[XmlAttribute("proxyPort")]
        //public int? ProxyPort { get; set; }
        
        ///// <summary>
        ///// The requested metadata
        ///// </summary>
        //[XmlAttribute("requestedMetadata")]
        //public ISHFieldMetadata RequestedMetadata { get; set; }

        ///// <summary>
        ///// The grouping metadata
        ///// </summary>
        //[XmlAttribute("groupingMetadata")]
        //public ISHFieldMetadata GroupingMetadata { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="TmsConfigurationSection"/> class from being created.
        /// </summary>
        private TmsConfigurationSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TmsConfigurationSection"/> class.
        /// </summary>
        /// <param name="alias">The alias of WorldServer</param>
        /// <param name="uri">The Uri to WorldServer </param>
        /// <param name="userName">The userName to get access to WorldServer</param>
        /// <param name="password">The password to get access to WorldServer</param>
        /// <param name="externalJobMaxTotalUncompressedSizeBytes">The max value of total size in bytes of uncompressed external job</param>
        /// <param name="retriesOnTimeout">The number of retries on timeout</param>
        /// <param name="mappings">The mapping between trisoftLanguage and worldServerLocaleId</param>
        /// <param name="templates">Tms templates</param>
        /// <param name="destinationPortNumber">The destination port number</param>
        /// <param name="isapiFilterLocation">The location of Isapi filter</param>
        /// <param name="useCompression">Use compression</param>
        /// <param name="useSsl">Use SSL</param>
        /// <param name="useDefaultProxyCredentials">Use default proxy credentials</param>
        /// <param name="proxyServer">The proxy server</param>
        /// <param name="proxyPort">The port number of proxy server</param>
        /// <param name="requestedMetadata">The port number of proxy server</param>
        /// <param name="groupingMetadata">The port number of proxy server</param>
        public TmsConfigurationSection(string alias, string uri, string userName, string password, int externalJobMaxTotalUncompressedSizeBytes, int retriesOnTimeout, ISHLanguageToTmsLanguageMapping[] mappings, TmsTemplate[] templates, int? destinationPortNumber = null, string isapiFilterLocation = null, bool? useCompression = null, bool? useSsl = null, bool? useDefaultProxyCredentials = null, string proxyServer = null, int? proxyPort = null, ISHFieldMetadata requestedMetadata = null, ISHFieldMetadata groupingMetadata = null)
        {
            Alias = alias;
            Uri = uri;
            UserName = userName;
            Password = password;
            ExternalJobMaxTotalUncompressedSizeBytes = externalJobMaxTotalUncompressedSizeBytes;
            RetriesOnTimeout = retriesOnTimeout;
            Mappings = mappings;
            Templates = templates;
            //DestinationPortNumber = destinationPortNumber;
            //IsapiFilterLocation = isapiFilterLocation;
            //UseCompression = useCompression;
            //UseSsl = useSsl;
            //UseDefaultProxyCredentials = useDefaultProxyCredentials;
            //ProxyServer = proxyServer;
            //ProxyPort = proxyPort;
            //RequestedMetadata = requestedMetadata;
            //GroupingMetadata = groupingMetadata;

            RelativeFilePath = @"TranslationOrganizer\Bin\TranslationOrganizer.exe.config";
            XPathToParentElement = "configuration/trisoft.infoShare.translationOrganizer/tms/instances";
            NameOfItem = "add";

            XPathFormat = "configuration/trisoft.infoShare.translationOrganizer/tms/instances/add[@alias='{0}']";
            XPath = string.Format(XPathFormat, alias);
        }
    }
}
