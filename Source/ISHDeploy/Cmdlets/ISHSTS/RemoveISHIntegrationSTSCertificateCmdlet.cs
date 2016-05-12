using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHSTS
{
    /// <summary>
    ///		<para type="synopsis">Removes STS certificate.</para>
    ///		<para type="description">The Remove-ISHIntegrationSTSCertificate cmdlet removes certificate based on a issuer name.</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.BaseHistoryEntryCmdlet" />
    /// <example>
    ///		<code>PS C:\&gt;Remove-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer "20151028ADFS"</code>
    ///		<para>This command removes STS trusted issuer credentials.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
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
			var operation = new RemoveISHIntegrationSTSCertificateOperation(Logger, Issuer);

			operation.Run();
		}
	}
}