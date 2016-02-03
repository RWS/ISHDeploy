using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor;
using InfoShare.Deployment.Models;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class DisableISHUIContentEditorCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNullOrEmpty]
        public ISHProject IshProject { get; set; }

        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new DisableISHUIContentEditorCmdSet(this, IshProject ?? ISHProjectProvider.Instance.IshProject);

            cmdSet.Run();
        }
    }
}
