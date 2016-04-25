using System.Linq;
using System.Management.Automation;
using System.ServiceModel.Security;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHSTS
{
	/// <summary>
	///		<para type="synopsis">Sets STS sertificate.</para>
	///		<para type="description">The Set-ISHIntegrationSTSCertificate cmdlet sets Thumbprint and issuers values to configuration.</para>
	/// </summary>
	/// <seealso cref="ISHDeploy.Cmdlets.BaseHistoryEntryCmdlet" />
	/// <example>
	///		<code>PS C:\&gt;Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "t1" -Issuer "20151028ADFS"</code>
	///		<para>This command sets STS trusted issuer credentials.
	/// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	/// <example>
	///		<code>PS C:\&gt;Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "t1" -Issuer "20151028ADFS" -ValidationMode "None" </code>
	///		<para>This command sets STS trusted issuer credentials with no Validation Mode.
	/// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	[Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSCertificate")]
	public sealed class SetISHIntegrationSTSCertificateCmdlet : BaseHistoryEntryCmdlet
	{
		/// <summary>
		/// Cashed and normalized Thumbprint value
		/// </summary>
		private string _thumbprint;

		/// <summary>
		/// <para type="description">Specifies the instance of the Content Manager deployment.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
		public Models.ISHDeployment ISHDeployment { get; set; }

		/// <summary>
		/// <para type="description">Certificate normalized Thumbprint.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Action of menu item")]
		[ValidateNotNullOrEmpty]
		public string Thumbprint
		{
			get
			{
				return _thumbprint; 
			}
			set
			{
				_thumbprint = new string(value.ToCharArray().Where(char.IsLetterOrDigit).ToArray());
			}
		}

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

			var operation = new SetISHIntegrationSTSCertificateOperation(Logger, Thumbprint, Issuer, ValidationMode);

			operation.Run();
		}
	}
}