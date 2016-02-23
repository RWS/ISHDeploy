using System.Management.Automation;
using InfoShare.Deployment.Data.Commands.FileCommands;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsCommon.Set, "ISHContentEditor", SupportsShouldProcess = false)]
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

        private ISHPaths _ishPaths;
        protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment));

        public override void ExecuteCmdlet()
		{
            var command = new FileCopyCommand(Logger, 
                LicensePath,
                IshPaths.LicenceFolderPath, 
                Force);

            command.Execute();
		}
	}
}