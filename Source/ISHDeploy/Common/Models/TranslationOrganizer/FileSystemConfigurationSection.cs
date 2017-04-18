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
    /// <para type="description">Represents configuration of an instance of FileSystem.</para>
    /// </summary>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("add", Namespace = "", IsNullable = false)]
    public class FileSystemConfigurationSection : BaseXMLElement
    {
        /// <summary>
        /// The alias of FileSystem 
        /// </summary>
        [XmlAttribute("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// The max value of total size in bytes of uncompressed external job
        /// </summary>
        [XmlAttribute("externalJobMaxTotalUncompressedSizeBytes")]
        public int ExternalJobMaxTotalUncompressedSizeBytes { get; set; }

        /// <summary>
        /// The path to export folder
        /// </summary>
        [XmlAttribute("exportFolder")]
        public string ExportFolderPath { get; set; }

        /// <summary>
        /// The requested metadata
        /// </summary>
        [XmlElement("requestedMetadata", Namespace = "")]
        public ISHMetadata RequestedMetadata { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="FileSystemConfigurationSection"/> class from being created.
        /// </summary>
        private FileSystemConfigurationSection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemConfigurationSection"/> class.
        /// </summary>
        /// <param name="alias">The alias of WorldServer</param>
        /// <param name="externalJobMaxTotalUncompressedSizeBytes">The max value of total size in bytes of uncompressed external job</param>
        /// <param name="exportFolderPath">The path to export folder</param>
        /// <param name="requestedMetadata">The port number of proxy server</param>
        public FileSystemConfigurationSection(string alias, int externalJobMaxTotalUncompressedSizeBytes, string exportFolderPath, ISHFieldMetadata[] requestedMetadata = null)
        {
            Alias = alias;
            ExternalJobMaxTotalUncompressedSizeBytes = externalJobMaxTotalUncompressedSizeBytes;
            ExportFolderPath = exportFolderPath;

            if (requestedMetadata != null)
            {
                RequestedMetadata = new ISHMetadata { Metadata = requestedMetadata };
            }

            RelativeFilePath = @"TranslationOrganizer\Bin\TranslationOrganizer.exe.config";
            XPathToParentElement = "configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances";
            NameOfItem = "add";

            XPathFormat = "configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add[@alias='{0}']";
            XPath = string.Format(XPathFormat, alias);
        }
    }
}
