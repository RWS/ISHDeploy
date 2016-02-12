using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(VerbsLifecycle.Enable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class EnableISHUIContentEditorCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNullOrEmpty]
        public Models.ISHDeployment ISHDeployment { get; set; }

        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new EnableISHUIContentEditorCmdSet(this, ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            cmdSet.Run();
        }
    }
}