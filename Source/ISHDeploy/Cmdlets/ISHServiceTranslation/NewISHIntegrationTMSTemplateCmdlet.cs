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
    /// <para type="synopsis">Creates a new ISH Template Field for TMS in Translation Organizer.</para>
    /// <para type="description">The New-ISHIntegrationTMSTemplate cmdlet creates an object with pair of properties: Template ID and Name. Used for tms translation integration.</para>
    /// <para type="link">Set-ISHIntegrationTMS</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>New-ISHIntegrationTMSTemplate -Name "testName" -TemplateId "testTemplateId"</code>
    /// <para>This command creates new object with metadata with name testName, level testLevel and valuetype testValueType.</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "ISHIntegrationTMSTemplate")]
    public sealed class NewISHIntegrationTMSTemplateCmdlet : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">The TMS template Id.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The TMS template Id.")]
        [ValidateNotNullOrEmpty]
        public string TemplateId { get; set; }

        /// <summary>
        /// <para type="description">The name of TMS template.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The name of TMS template.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var tmsTemplate = new TmsTemplate
            {
                TemplateId = TemplateId,
                TemplateName = Name
            };

            var result = new List<TmsTemplate> { tmsTemplate };
            ISHWriteOutput(result);
        }
    }
}
