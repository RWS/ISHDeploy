using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHExternalPreview
{
    [Cmdlet(VerbsLifecycle.Enable, "ISHExternalPreview", SupportsShouldProcess = false)]
    public sealed class EnableISHExternalPreviewCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        [Alias("extrId")]
        [ValidateNotNull]
        public string ExternalId { get; set; }

        public override void ExecuteCmdlet()
        {
            var ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            var cmdSet = new EnableISHExternalPreviewCmdSet(this, ishPaths, ExternalId);

            cmdSet.Run();
        }
    }
}
