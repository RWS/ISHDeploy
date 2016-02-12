using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHExternalPreview
{
    [Cmdlet(VerbsLifecycle.Disable, "ISHExternalPreview", SupportsShouldProcess = false)]
    public sealed class DisableISHExternalPreviewCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNullOrEmpty]
        public Models.ISHDeployment ISHDeployment { get; set; }

        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new DisableISHExternalPreviewCmdSet(this, ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            cmdSet.Run();
        }
    }
}
