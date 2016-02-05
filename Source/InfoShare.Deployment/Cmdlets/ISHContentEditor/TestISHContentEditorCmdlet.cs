using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHContentEditor;
using InfoShare.Deployment.Models;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsDiagnostic.Test, "ISHContentEditor", SupportsShouldProcess = false)]
	public sealed class TestISHContentEditorCmdlet : BaseCmdlet
	{
		[Parameter(Mandatory = true, Position = 0, HelpMessage = "Path to the license file")]
		[Alias("path")]
		[ValidateNotNullOrEmpty]
		public string Hostname { get; set; }

		[Parameter(Mandatory = false, Position = 2)]
		[Alias("proj")]
		[ValidateNotNullOrEmpty]
		public ISHProject IshProject { get; set; }

		public override void ExecuteCmdlet()
		{
			bool result = false;

			var cmdSet = new TestISHContentEditorCmdSet(this, IshProject ?? ISHProjectProvider.Instance.IshProject, Hostname,
				isValid =>
				{
					result = isValid;
				});
			cmdSet.Run();

			WriteObject(result);
		}
	}
}