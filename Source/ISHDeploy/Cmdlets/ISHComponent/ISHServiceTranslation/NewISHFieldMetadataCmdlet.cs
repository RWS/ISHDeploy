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
using System.Management.Automation;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets.ISHComponent.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Creates a new ISH Metadata Field for TMS in Translation Organizer.</para>
    /// <para type="description">The New-ISHFieldMetadata cmdlet creates an object that represents structure of ISH metadata field for organizer service.</para>
    /// <para type="link">Set-ISHServiceTranslationOrganizer</para>
    /// <para type="link">Set-ISHIntegrationTMS</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>New-ISHFieldMetadata -Name "testName" -Level "logical" -ValueType "value"</code>
    /// <para>This command creates new object with metadata with name testName, level "logical" and valuetype "value".</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "ISHFieldMetadata")]
    public sealed class NewISHFieldMetadataCmdlet  : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">The name of ISHMetadata field.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The name of ISHMetadata field")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The Level of ISHMetadata field.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The Level of ISHMetadata field")]
        [ValidateNotNullOrEmpty]
        public ISHFieldMetadataLevel Level { get; set; }

        /// <summary>
        /// <para type="description">The Type of value of ISHMetadata field.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The Type of value of ISHMetadata field")]
        [ValidateNotNullOrEmpty]
        public ISHFieldMetadataValueType ValueType { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var ishFieldMetadata = new ISHFieldMetadata
            {
                Name = Name,
                Level = Level,
                ValueType = ValueType
            };

            var result = new List<ISHFieldMetadata> { ishFieldMetadata };
            ISHWriteOutput(result);
        }
    }
}
