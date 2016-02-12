using System.IO;
using System.Management.Automation;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.FileCommands;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsCommon.Set, "ISHContentEditor", SupportsShouldProcess = false)]
	public sealed class SetISHContentEditorCmdlet : BaseCmdlet
	{
		[Parameter(Mandatory = true, Position = 0, HelpMessage = "Path to the license file")]
        [Alias("path")]
		[ValidateNotNullOrEmpty]
		public string LicensePath { get; set; }

		[Parameter(Mandatory = false, Position = 1)]
		public SwitchParameter Force { get; set; }

		[Parameter(Mandatory = false, Position = 2)]
        [Alias("proj")]
		[ValidateNotNullOrEmpty]
		public Models.ISHDeployment ISHDeployment { get; set; }

		public override void ExecuteCmdlet()
		{
		    var ishProject = ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment;

            var command = new FileCopyCommand(this, 
                LicensePath,
		        Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LicenceFolderPath), 
                Force);

            command.Execute();
		}
	}
}