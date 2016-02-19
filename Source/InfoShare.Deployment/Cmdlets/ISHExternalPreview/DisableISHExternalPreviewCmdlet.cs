using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHExternalPreview
{
    [Cmdlet(VerbsLifecycle.Disable, CmdletNames.ISHExternalPreview, SupportsShouldProcess = false)]
    public sealed class DisableISHExternalPreviewCmdlet : BaseHistoryEntryCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        protected override string HistoryEntry => $"{VerbsLifecycle.Disable}-{CmdletNames.ISHExternalPreview}";

        protected override string DeploymentSuffix => (ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment).Suffix;

        public override void ExecuteCmdlet()
        {
            var ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            var cmdSet = new DisableISHExternalPreviewCmdSet(Logger, ishPaths);

            cmdSet.Run();
        }
    }
}
