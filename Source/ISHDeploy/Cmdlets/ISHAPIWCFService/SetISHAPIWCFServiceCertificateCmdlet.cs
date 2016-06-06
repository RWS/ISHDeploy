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
    ///         Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
	/// <example>
	///		<code>PS C:\&gt;Set-ISHAPIWCFServiceCertificate -ISHDeployment $deployment -Thumbprint "t1" -ValidationMode "None" </code>
	///		<para>This command sets credentials with no Validation Mode.
	/// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
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
