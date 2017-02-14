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
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Creates a new mapping pair ISHLanguage to TmsLanguage.</para>
    /// <para type="description">The New-ISHIntegrationTMSMapping cmdlet creates an object with pair of properties, where ISHLanguage is InfoShare language identifier (example: "en") and TmsLanguage is language identifier of TMS (example: "EN-US").</para>
    /// <para type="link">Set-ISHIntegrationTMS</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>New-ISHIntegrationTMSMapping -ISHLanguage "en" -TmsLanguage "EN-US"</code>
    /// <para>This command creates new object for English language where ID of the same language on TMS is EN-US.</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "ISHIntegrationTMSMapping")]
    public sealed class NewISHIntegrationTMSMappingCmdlet : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">The language identifier of TMS.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The language identifier of TMS")]
        [ValidateNotNullOrEmpty]
        public string TmsLanguage { get; set; }

        /// <summary>
        /// <para type="description">The trisoft language identifier.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The trisoft language identifier")]
        [ValidateNotNullOrEmpty]
        public string ISHLanguage { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var ishLanguageToTmsLocaleIdMapping = new ISHLanguageToTmsLanguageMapping
            {
                TmsLanguage = TmsLanguage,
                ISHLanguage = ISHLanguage
            };

            var result = new List<ISHLanguageToTmsLanguageMapping> { ishLanguageToTmsLocaleIdMapping };
            ISHWriteOutput(result);
        }
    }
}
