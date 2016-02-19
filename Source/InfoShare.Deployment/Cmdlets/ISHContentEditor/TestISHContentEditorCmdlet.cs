using System.Management.Automation;
using InfoShare.Deployment.Data.Commands.LicenseCommands;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsDiagnostic.Test, CmdletNames.ISHContentEditor, SupportsShouldProcess = false)]
	public sealed class TestISHContentEditorCmdlet : BaseCmdlet
	{
		[Parameter(Mandatory = true, Position = 0, HelpMessage = "Path to the license file")]
		[Alias("path")]
		[ValidateNotNullOrEmpty]
		public string Hostname { get; set; }

		[Parameter(Mandatory = false, Position = 2)]
		[Alias("proj")]
		[ValidateNotNull]
		public Models.ISHDeployment ISHDeployment { get; set; }

		public override void ExecuteCmdlet()
		{
			bool result = false;

            var ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            var command = new LicenseTestCommand(
		        Logger,
		        ishPaths.LicenceFolderPath,
		        Hostname,
		        isValid =>
		        {
		            result = isValid;
		        });

            command.Execute();

            WriteObject(result);
		}
	}
}