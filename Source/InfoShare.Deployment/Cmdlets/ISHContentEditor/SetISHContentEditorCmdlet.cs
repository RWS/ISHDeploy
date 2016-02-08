using System.IO;
using System.Management.Automation;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.FileCommands;
using InfoShare.Deployment.Models;

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
		public ISHProject IshProject { get; set; }

		public override void ExecuteCmdlet()
		{
		    var command = new FileCopyCommand(this, 
                LicensePath,
		        Path.Combine(IshProject.AuthorFolderPath, ISHPaths.LicenceFolderPath), 
                Force);

            command.Execute();
		}
	}
}