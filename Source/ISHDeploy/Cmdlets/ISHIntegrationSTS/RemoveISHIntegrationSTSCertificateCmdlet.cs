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
using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTS
{
    /// <summary>
    ///		<para type="synopsis">Removes STS certificate.</para>
    ///		<para type="description">The Remove-ISHIntegrationSTSCertificate cmdlet removes certificate based on a issuer name.</para>
    ///		<para type="link">Disable-ISHIntegrationSTSInternalAuthenticationCmdlet</para>
    ///     <para type="link">Enable-ISHIntegrationSTSInternalAuthenticationCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSCertificateCmdlet</para>
    ///     <para type="link">Save-ISHIntegrationSTSConfigurationPackageCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSFederationCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSTrustCmdlet</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.ISHIntegrationSTS" />
    /// <example>
    ///		<code>PS C:\&gt;Remove-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer "20151028ADFS"</code>
    ///		<para>This command removes STS trusted issuer credentials.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "ISHIntegrationSTSCertificate")]
	public sealed class RemoveISHIntegrationSTSCertificateCmdlet : BaseHistoryEntryCmdlet
	{
		/// <summary>
		/// <para type="description">Issuer name.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Issuer name")]
		[ValidateNotNullOrEmpty]
		public string Issuer { get; set; }

		/// <summary>
		/// Executes cmdlet
		/// </summary>
		public override void ExecuteCmdlet()
		{
			var operation = new RemoveISHIntegrationSTSCertificateOperation(Logger, ISHDeployment, Issuer);

			operation.Run();
		}
	}
}