using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHContentEditor;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsDiagnostic.Test, "ISHContentEditor", SupportsShouldProcess = false)]
	public sealed class TestISHContentEditorCmdlet : BaseCmdlet
	{
		public override void ExecuteCmdlet()
		{
			// Calling the command directly
			//bool result = false;
			//var command = new TestISHContentEditorCmdSet(this, exists => result = exists);
			//command.Execute();

			//WriteObject(result);
		}
	}
}