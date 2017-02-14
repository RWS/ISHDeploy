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
using System.Runtime.InteropServices;
using ISHDeploy.Business.Operations.ISHServiceTranslation;
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
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHIntegrationTMS -ISHDeployment $deployment -Name "ws1" -Uri "https:\\ws1.sd.com" -Credential $credential -MaximumJobSize 5242880 -RetriesOnTimeout 3 -Mapping $mapping -Templates $templates</code>
    /// <para>This command enables the translation organizer windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    /// Parameter $credential is a set of security credentials, such as a user name and a password.
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
        /// <para type="description">The credential to get access to TMS.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The credential to get access to TMS")]
        [ValidateNotNullOrEmpty]
        public PSCredential Credential { get; set; }

        /// <summary>
        /// <para type="description">The max value of total size in bytes of uncompressed external job.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The max value of total size in bytes of uncompressed external job")]
        [ValidateNotNullOrEmpty]
        public int MaximumJobSize { get; set; }

        /// <summary>
        /// <para type="description">The number of retries on timeout.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The number of retries on timeout.")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 30)]
        public int RetriesOnTimeout { get; set; }

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
        /// <para type="description">The destination port number.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The destination port number.")]
        public int DestinationPortNumber { get; set; }

        /// <summary>
        /// <para type="description">The location of Isapi filter.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The location of Isapi filter.")]
        public string IsapiFilterLocation { get; set; }

        /// <summary>
        /// <para type="description">Use compression</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Use compression")]
        public bool UseCompression { get; set; }

        /// <summary>
        /// <para type="description">Use SSL.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Use SSL.")]
        public bool UseSsl { get; set; }

        /// <summary>
        /// <para type="description">Use default proxy credentials.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Use default proxy credentials.")]
        public bool UseDefaultProxyCredentials { get; set; }

        /// <summary>
        /// <para type="description">The proxy server.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The proxy server.")]
        public string ProxyServer { get; set; }

        /// <summary>
        /// <para type="description">The port number of proxy server.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The port number of proxy server.")]
        public int ProxyPort { get; set; }

        /// <summary>
        /// <para type="description">The requested metadata.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The requested metadata.")]
        public ISHFieldMetadata RequestedMetadata { get; set; }

        /// <summary>
        /// <para type="description">The grouping metadata.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The grouping metadata.")]
        public ISHFieldMetadata GroupingMetadata { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var tmsConfiguration = new TmsConfigurationSection(
                Name,
                Uri,
                Credential.UserName,
                Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Credential.Password)),
                MaximumJobSize,
                RetriesOnTimeout,
                Mappings,
                Templates,
                RequestedMetadata,
                GroupingMetadata);

            var parameters = new Dictionary<TmsConfigurationSetting, object>();
            foreach (var cmdletParameter in MyInvocation.BoundParameters)
            {
                switch (cmdletParameter.Key)
                {
                    case "DestinationPortNumber":
                        parameters.Add(TmsConfigurationSetting.destinationPortNumber, DestinationPortNumber);
                        break;
                    case "IsapiFilterLocation":
                        parameters.Add(TmsConfigurationSetting.isapiFilterLocation, IsapiFilterLocation);
                        break;
                    case "UseCompression":
                        parameters.Add(TmsConfigurationSetting.useCompression, UseCompression);
                        break;
                    case "UseSsl":
                        parameters.Add(TmsConfigurationSetting.useSsl, UseSsl);
                        break;
                    case "UseDefaultProxyCredentials":
                        parameters.Add(TmsConfigurationSetting.useDefaultProxyCredentials, UseDefaultProxyCredentials);
                        break;
                    case "ProxyServer":
                        parameters.Add(TmsConfigurationSetting.proxyServer, ProxyServer);
                        break;
                    case "ProxyPort":
                        parameters.Add(TmsConfigurationSetting.proxyPort, ProxyPort);
                        break;
                }
            }

            var operation = new SetISHIntegrationTmsOperation(
                Logger, 
                ISHDeployment, 
                tmsConfiguration,
                parameters);

            operation.Run();
        }
    }
}
