using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor;
using InfoShare.Deployment.Models;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(VerbsLifecycle.Enable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class EnableISHUIContentEditorCmdlet : BaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new EnableISHUIContentEditorCmdSet(this, ISHProjectProvider.Instance.IshProject);

            cmdSet.Run();
        }
    }
}