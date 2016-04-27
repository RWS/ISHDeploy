using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHSTS;
using ISHDeploy.Validators;

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
		/// <para type="description">Specifies the instance of the Content Manager deployment.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        [ValidateDeploymentVersion]
        public Models.ISHDeployment ISHDeployment { get; set; }

		/// <summary>
		/// <para type="description">Issuer name.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Issuer name")]
		[ValidateNotNullOrEmpty]
		public string Issuer { get; set; }

		/// <summary>
		/// Cashed value for <see cref="IshPaths"/> property
		/// </summary>
		private ISHPaths _ishPaths;

		/// <summary>
		/// Returns instance of the <see cref="ISHPaths"/>
		/// </summary>
		protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

		/// <summary>
		/// Executes cmdlet
		/// </summary>
		public override void ExecuteCmdlet()
		{
			OperationPaths.Initialize(ISHDeployment);

			var operation = new RemoveISHIntegrationSTSCertificateOperation(Logger, Issuer);

			operation.Run();
		}
	}
}