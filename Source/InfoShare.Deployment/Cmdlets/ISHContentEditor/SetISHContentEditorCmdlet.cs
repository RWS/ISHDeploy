using System.Management.Automation;
using InfoShare.Deployment.Data.Commands.FileCommands;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsCommon.Set, CmdletNames.ISHContentEditor, SupportsShouldProcess = false)]
	public sealed class SetISHContentEditorCmdlet : BaseHistoryEntryCmdlet
	{
		[Parameter(Mandatory = true, Position = 0, HelpMessage = "Path to the license file")]
        [Alias("path")]
		[ValidateNotNullOrEmpty]
		public string LicensePath { get; set; }

		[Parameter(Mandatory = false, Position = 1)]
		public SwitchParameter Force { get; set; }

		[Parameter(Mandatory = false, Position = 2)]
        [Alias("proj")]
		[ValidateNotNull]
		public Models.ISHDeployment ISHDeployment { get; set; }

	    protected override string HistoryEntry => $"{VerbsCommon.Set}-{CmdletNames.ISHContentEditor} -{nameof(LicensePath)} {LicensePath}";

        protected override string DeploymentSuffix => (ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment).Suffix;

        public override void ExecuteCmdlet()
		{
            var ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            var command = new FileCopyCommand(Logger, 
                LicensePath,
                ishPaths.LicenceFolderPath, 
                Force);

            command.Execute();
		}
	}
}