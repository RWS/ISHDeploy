using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHExternalPreview
{
    [Cmdlet(VerbsLifecycle.Disable, "ISHExternalPreview", SupportsShouldProcess = false)]
    public sealed class DisableISHExternalPreviewCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        public override void ExecuteCmdlet()
        {
            var ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            var cmdSet = new DisableISHExternalPreviewCmdSet(Logger, ishPaths);

            cmdSet.Run();
        }
    }
}
