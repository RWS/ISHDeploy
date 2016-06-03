using System.Management.Automation;
using System.ServiceModel.Security;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHAPIWCFService;

namespace ISHDeploy.Cmdlets.ISHAPIWCFService
{
    /// <summary>
    ///		<para type="synopsis">Sets WSFederation configuration.</para>
    ///		<para type="description">The Set-ISHIntegrationSTSWSFederation cmdlet sets WSFederation configuration to Content Manager deployment.</para>
    /// </summary>
    /// <example>
    ///		<code>PS C:\>Set-ISHIntegrationSTSWSFederation -ISHDeployment $deployment -Endpoint "https://test.global.sdl.corp/InfoShareSTS/issue/wsfed"</code>
    ///     <para>This command configure WS Federation to use specified Endpoint of STS server.
    ///         Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
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
