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
﻿using ISHDeploy.Business.Enums;
﻿using ISHDeploy.Business.Operations.ISHIntegrationSTS;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTS
{
    /// <summary>
    ///		<para type="synopsis">Sets WSTrust configuration.</para>
    ///		<para type="description">The Set-ISHIntegrationSTSWSTrust cmdlet sets WSTrust configuration to Content Manager deployment.</para>
    ///		<para type="description">When -IncludeInternalClients is switched on then the -ActorUsername and -ActorPassword must be specified.</para>
    ///		<para type="link">Disable-ISHIntegrationSTSInternalAuthentication</para>
    ///     <para type="link">Enable-ISHIntegrationSTSInternalAuthentication</para>
    ///     <para type="link">Remove-ISHIntegrationSTSCertificate</para>
    ///     <para type="link">Set-ISHIntegrationSTSCertificate</para>
    ///     <para type="link">Save-ISHIntegrationSTSConfigurationPackage</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSFederation</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.ISHIntegrationSTS" />
    /// <example>
    ///		<code>PS C:\>Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment -Endpoint "https://adfs.example.com/adfs/services/trust/13/windowsmixed" -MexEndpoint "https://adfs.example.com/adfs/services/trust/mex" -BindingType "WindowsMixed" -Verbose</code>
    ///     <para>This command configure WS to use specified Endpoint and MexEndpoint of STS server and sets type of authentication as WindowsMixed.
    ///         Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    /// <example>
    ///		<code>PS C:\>Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment -Endpoint "https://adfs.example.com/adfs/services/trust/13/windowsmixed" -MexEndpoint "https://adfs.example.com/adfs/services/trust/mex" -BindingType "WindowsMixed" -IncludeInternalClients -Verbose</code>
    ///     <para>This command configure WS to use specified Endpoint and MexEndpoint of STS server, sets type of authentication as WindowsMixed and sets internal clients credentials.
    ///         Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSWSTrust")]
    [AdministratorRights]
    public class SetISHIntegrationSTSWSTrustCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the URL to issuer WSTrust endpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust endpoint")]
        public Uri Endpoint { get; set; }

        /// <summary>
        /// <para type="description">Specifies the URL to issuer WSTrust mexEndpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust mexEndpoint")]
        public Uri MexEndpoint { get; set; }

        /// <summary>
        /// <para type="description">Specifies the STS issuer authentication type.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Type of STS issuer authentication")]
        public BindingType BindingType { get; set; }

        /// <summary>
        /// <para type="description">Specifies that STS -ActorUsername and -ActorPassword need to be updated.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "When -IncludeInternalClients is switched on then -ActorUsername and -ActorPassword have meaning")]
        public SwitchParameter IncludeInternalClients { get; set; }

        /// <summary>
        /// <para type="description">Specifies the STS actor Username.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Username of STS actor")]
        public string ActorUsername { get; set; }

        /// <summary>
        /// <para type="description">Specifies the STS actor password.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Password of STS actor")]
        public string ActorPassword { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (MyInvocation.BoundParameters.ContainsKey("IncludeInternalClients"))
            {

                if (MyInvocation.BoundParameters.ContainsKey("ActorUsername") && ActorUsername == null)
                {
                    ActorUsername = string.Empty;
                }

                if (MyInvocation.BoundParameters.ContainsKey("ActorPassword") && ActorPassword == null)
                {
                    ActorPassword = string.Empty;
                }

                var operation = new SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation(Logger, ISHDeployment, Endpoint, MexEndpoint, BindingType, ActorUsername, ActorPassword);
                operation.Run();
            }
            else
            {
                var operation = new SetISHIntegrationSTSWSTrustOperation(Logger, ISHDeployment, Endpoint, MexEndpoint, BindingType);
                operation.Run();
            }
        }
    }
}
