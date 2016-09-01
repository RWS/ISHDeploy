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
﻿using System.Management.Automation;
﻿using ISHDeploy.Business.Enums;
﻿using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHSTS
{
    /// <summary>
    ///		<para type="synopsis">Sets STS token signing certificate and/or type of authentication.</para>
    ///		<para type="description">The Set-ISHIntegrationSTSCertificate cmdlet sets STS token signing certificate and/or type of authentication.</para>
    ///     <para type="link">Get-ISHSTSRelyingPartyCmdlet</para>
    ///     <para type="link">Remove-ISHIntegrationSTSCertificateCmdlet</para>
    ///     <para type="link">Reset-ISHSTSCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSCertificateCmdlet</para>
    ///     <para type="link">Set-ISHSTSRelyingPartyCmdlet</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.BaseHistoryEntryCmdlet" />
    /// <example>
    ///		<code>PS C:\&gt;Set-ISHSTSConfiguration -ISHDeployment $deployment -TokenSigningCertificateThumbprint "t1"</code>
    ///		<para>This command sets STS token signing certificate.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    /// <example>
    ///		<code>PS C:\&gt;Set-ISHSTSConfiguration -ISHDeployment $deployment -AuthenticationType "Windows"</code>
    ///		<para>This command sets Windows Authentication type for STS.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHSTSConfiguration")]
	public sealed class SetISHSTSConfigurationCmdlet : BaseHistoryEntryCmdlet
	{
        /// <summary>
        /// <para type="description">Token signing certificate Thumbprint.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Token signing certificate Thumbprint")]
		[ValidateNotNullOrEmpty]
		public string TokenSigningCertificateThumbprint { get; set; }

        /// <summary>
        /// <para type="description">Selected authentication type.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Selected authentication type")]
		public AuthenticationTypes AuthenticationType { get; set; }

		/// <summary>
		/// Executes cmdlet
		/// </summary>
		public override void ExecuteCmdlet()
		{

            if (MyInvocation.BoundParameters.ContainsKey("TokenSigningCertificateThumbprint") && MyInvocation.BoundParameters.ContainsKey("AuthenticationType"))
            {
                var operation = new SetISHSTSConfigurationOperation(Logger, ISHDeployment, TokenSigningCertificateThumbprint, AuthenticationType);
                operation.Run();
            }
            else if (MyInvocation.BoundParameters.ContainsKey("TokenSigningCertificateThumbprint") && !MyInvocation.BoundParameters.ContainsKey("AuthenticationType"))
            {
                var operation = new SetISHSTSConfigurationOperation(Logger, ISHDeployment, TokenSigningCertificateThumbprint);
                operation.Run();
            }
            else if (!MyInvocation.BoundParameters.ContainsKey("TokenSigningCertificateThumbprint") && MyInvocation.BoundParameters.ContainsKey("AuthenticationType"))
            {
                var operation = new SetISHSTSConfigurationOperation(Logger, ISHDeployment, AuthenticationType);
                operation.Run();
            }
            else
            {
                Logger.WriteWarning("Set-ISHSTSConfiguration cmdlet has no parameters to set");
            }
		}
	}
}