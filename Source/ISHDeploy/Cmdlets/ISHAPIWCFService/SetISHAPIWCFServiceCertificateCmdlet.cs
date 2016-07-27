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
using System.Management.Automation;
using System.ServiceModel.Security;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHAPIWCFService;

namespace ISHDeploy.Cmdlets.ISHAPIWCFService
{
    /// <summary>
    ///		<para type="synopsis">Sets WCF service certificate.</para>
    ///		<para type="description">The Set-ISHAPIWCFServiceCertificate cmdlet sets WCF service certificate to Content Manager deployment.</para>
    /// </summary>
    /// <example>
    ///		<code>PS C:\>Set-ISHAPIWCFServiceCertificate -ISHDeployment $deployment -Thumbprint "t1"</code>
    ///     <para>This command configure WCF service to use specified certificate.
    ///         Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
	/// <example>
	///		<code>PS C:\&gt;Set-ISHAPIWCFServiceCertificate -ISHDeployment $deployment -Thumbprint "t1" -ValidationMode "None" </code>
	///		<para>This command configure WCF service to use specified certificate and sets credentials with no Validation Mode.
	///         Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
    [Cmdlet(VerbsCommon.Set, "ISHAPIWCFServiceCertificate")]
    public class SetISHAPIWCFServiceCertificateCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Certificate Thumbprint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The Thumbprint of WCF service certificate")]
        [ValidateNotNullOrEmpty]
        public string Thumbprint { get; set; }

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
            IOperation operation = new SetISHAPIWCFServiceCertificateOperation(Logger, ISHDeployment, Thumbprint, ValidationMode);

            operation.Run();
        }
    }
}
