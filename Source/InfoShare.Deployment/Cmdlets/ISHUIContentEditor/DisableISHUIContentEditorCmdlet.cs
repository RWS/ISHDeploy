using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class DisableISHUIContentEditorCmdlet : BaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new DisableISHUIContentEditorCmdSet(this, ISHProjectProvider.Instance.IshProject);

            cmdSet.Run();
        }
    }
}
