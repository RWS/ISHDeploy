/**
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
using System.ServiceModel.Security;
using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHSTS
{
	/// <summary>
	///		<para type="synopsis">Sets STS certificate.</para>
	///		<para type="description">The Set-ISHIntegrationSTSCertificate cmdlet sets Thumbprint and issuers values to configuration.</para>
	/// </summary>
	/// <seealso cref="ISHDeploy.Cmdlets.BaseHistoryEntryCmdlet" />
	/// <example>
	///		<code>PS C:\&gt;Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "t1" -Issuer "20151028ADFS"</code>
	///		<para>This command sets STS trusted issuer credentials.
	/// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	/// <example>
	///		<code>PS C:\&gt;Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "t1" -Issuer "20151028ADFS" -ValidationMode "None" </code>
	///		<para>This command sets STS trusted issuer credentials with no Validation Mode.
	/// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	[Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSCertificate")]
	public sealed class SetISHIntegrationSTSCertificateCmdlet : BaseHistoryEntryCmdlet
	{
		/// <summary>
		/// <para type="description">Certificate Thumbprint.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Certificate Thumbprint")]
		[ValidateNotNullOrEmpty]
		public string Thumbprint { get; set; }

        /// <summary>
        /// <para type="description">Issuer name.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Issuer name")]
		[ValidateNotNullOrEmpty]
		public string Issuer { get; set; }
		
		/// <summary>
		/// <para type="description">Selected validation mode. Default value is 'ChainTrust'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Selected validation mode")]
		public X509CertificateValidationMode ValidationMode { get; set; } = X509CertificateValidationMode.ChainTrust;

		/// <summary>
		/// Executes cmdlet
		/// </summary>
		public override void ExecuteCmdlet()
		{
			var operation = new SetISHIntegrationSTSCertificateOperation(Logger, ISHDeployment, Thumbprint, Issuer, ValidationMode);

			operation.Run();
		}
	}
}