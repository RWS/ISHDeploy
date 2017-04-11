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
using System.Collections.Generic;
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHServiceTranslation;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets.ISHServiceTranslation
{
    /// <summary>
    /// <para type="synopsis">Sets configuration of TMS.</para>
    /// <para type="description">The Set-ISHIntegrationTMS cmdlet sets configuration of TMS.</para>
    /// <para type="link">New-ISHIntegrationTMSMapping</para>
    /// <para type="link">New-ISHIntegrationTMSTemplate</para>
    /// <para type="link">New-ISHFieldMetadata</para>
    /// <para type="link">Remove-ISHIntegrationTMS</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHIntegrationTMS -ISHDeployment $deployment -Name "ws1" -Uri "https:\\ws1.sd.com" -MaximumJobSize 5242880 -RetriesOnTimeout 3 -ApiKey "someApiKey" -SecretKey "someSecretKey" -Mapping $mapping -Templates $templates</code>
    /// <para>This command enables the translation organizer windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    /// Parameter $mapping is a object with pair of properties, where ISHLanguage is InfoShare language identifier retrieved from New-ISHIntegrationTMSMapping cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationTMS")]
    public sealed class SetISHIntegrationTMSCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The name (alias) of TMS.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The name (alias) of TMS")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The Uri to TMS.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The Uri to TMS")]
        [ValidateNotNullOrEmpty]
        public string Uri { get; set; }

        /// <summary>
        /// <para type="description">The max value of total size in bytes of uncompressed external job.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The max value of total size in bytes of uncompressed external job")]
        public int MaximumJobSize { get; set; } = 5242880;

        /// <summary>
        /// <para type="description">The number of retries on timeout.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The number of retries on timeout.")]
        [ValidateRange(1, 30)]
        public int RetriesOnTimeout { get; set; } = 3;

        /// <summary>
        /// <para type="description">The mapping between ISHLanguage and TmsLanguage.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The mapping between trisoftLanguage and TmsLanguage.")]
        [ValidateNotNullOrEmpty]
        public ISHLanguageToTmsLanguageMapping[] Mappings { get; set; }

        /// <summary>
        /// <para type="description">The TMS templates.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The mapping between trisoftLanguage and TmsLanguage.")]
        [ValidateNotNullOrEmpty]
        public TmsTemplate[] Templates { get; set; }

        /// <summary>
        /// <para type="description">The requested metadata.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The requested metadata.")]
        public ISHFieldMetadata[] RequestMetadata { get; set; }

        /// <summary>
        /// <para type="description">The grouping metadata.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The grouping metadata.")]
        public ISHFieldMetadata[] GroupMetadata { get; set; }

        /// <summary>
        /// <para type="description">API key that is associated with a user.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "API key that is associated with a user")]
        [ValidateNotNullOrEmpty]
        public string ApiKey { get; set; }

        /// <summary>
        /// <para type="description">Secret - API key encrypted using HMACSHA256 with the password.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Secret - API key encrypted using HMACSHA256 with the password")]
        [ValidateNotNullOrEmpty]
        public string SecretKey { get; set; }

        /// <summary>
        /// <para type="description">The HTTP timeout.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The timeout used when doing the SDL TMS call using REST API. Default value is 00:02:00.000.")]
        [ValidateNotNullOrEmpty]
        public TimeSpan HttpTimeout { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var tmsConfiguration = new TmsConfigurationSection(
                Name,
                Uri,
                MaximumJobSize,
                RetriesOnTimeout,
                Mappings,
                Templates,
                ApiKey,
                SecretKey,
                RequestMetadata,
                GroupMetadata);

            var parameters = new Dictionary<TmsConfigurationSetting, object>();

            if (MyInvocation.BoundParameters.ContainsKey("HttpTimeout"))
            {
                parameters.Add(TmsConfigurationSetting.httpTimeout, HttpTimeout);
            }
            
            var operation = new SetISHIntegrationTmsOperation(
                Logger, 
                ISHDeployment, 
                tmsConfiguration,
                MyInvocation.BoundParameters.ContainsKey("MaximumJobSize"),
                MyInvocation.BoundParameters.ContainsKey("RetriesOnTimeout"),
                parameters,
                "TranslationOrganizer.exe.config already contains settings for TMS server. You should remove TMS configuration section first. To do this you can use Remove-ISHIntegrationTMS cmdlet.");

            operation.Run();
        }
    }
}
