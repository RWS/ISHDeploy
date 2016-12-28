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
﻿using System;
using System.Management.Automation;
﻿using ISHDeploy.Business.Operations.ISHIntegrationSTS;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTS
{
    /// <summary>
    ///		<para type="synopsis">Sets WSFederation configuration.</para>
    ///		<para type="description">The Set-ISHIntegrationSTSWSFederation cmdlet sets WSFederation configuration to Content Manager deployment.</para>
    ///		<para type="link">Disable-ISHIntegrationSTSInternalAuthentication</para>
    ///     <para type="link">Enable-ISHIntegrationSTSInternalAuthentication</para>
    ///     <para type="link">Remove-ISHIntegrationSTSCertificate</para>
    ///     <para type="link">Set-ISHIntegrationSTSCertificate</para>
    ///     <para type="link">Save-ISHIntegrationSTSConfigurationPackage</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSTrust</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.ISHIntegrationSTS" />
    /// <example>
    ///		<code>PS C:\>Set-ISHIntegrationSTSWSFederation -ISHDeployment $deployment -Endpoint "https://adfs.example.com/adfs/ls/"</code>
    ///     <para>This command configure WS Federation to use specified Endpoint of STS server.
    ///         Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSWSFederation")]
    public class SetISHIntegrationSTSWSFederationCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the URL to issuer endpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSFederation endpoint")]
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new SetISHIntegrationSTSWSFederationOperation(Logger, ISHDeployment, Endpoint);

            operation.Run();
        }
    }
}
