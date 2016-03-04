using System.Management.Automation;
using InfoShare.Deployment.Data.Actions.License;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsDiagnostic.Test, "ISHContentEditor", SupportsShouldProcess = false)]
	public sealed class TestISHContentEditorCmdlet : BaseCmdlet
	{
		[Parameter(Mandatory = true, Position = 0, HelpMessage = "Path to the license file")]
		[Alias("path")]
		[ValidateNotNullOrEmpty]
		public string Domain { get; set; }

		[Parameter(Mandatory = false, Position = 2)]
		[Alias("proj")]
		[ValidateNotNull]
		public Models.ISHDeployment ISHDeployment { get; set; }

		public override void ExecuteCmdlet()
		{
			bool result = false;

            var ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            var action = new LicenseTestAction(
		        Logger,
		        ishPaths.LicenceFolderPath,
				Domain,
		        isValid =>
		        {
		            result = isValid;
		        });

            action.Execute();

            WriteObject(result);
		}
	}
}