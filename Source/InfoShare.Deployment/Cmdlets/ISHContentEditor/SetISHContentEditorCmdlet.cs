using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHContentEditor;
using InfoShare.Deployment.Models;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsCommon.Set, "ISHContentEditor", SupportsShouldProcess = false)]
	public sealed class SetISHContentEditorCmdlet : BaseCmdlet
	{
		[Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup", HelpMessage = "Path to the license file")]
		[ValidateNotNullOrEmpty]
		public string LicensePath { get; set; }

		[Parameter(Mandatory = false, Position = 2, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
		public bool Force { get; set; }

		[Parameter(Mandatory = false, Position = 3, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
		[ValidateNotNullOrEmpty]
		public ISHProject IshProject { get; set; }

		public override void ExecuteCmdlet()
		{
			// Calling the set of command with entry parameters
			var cmdSet = new SetISHContentEditorCmdSet(this, IshProject ?? ISHProjectProvider.Instance.IshProject, LicensePath, Force);
			cmdSet.Run();
		}
	}
}