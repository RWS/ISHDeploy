using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHExternalPreview
{
    [Cmdlet(VerbsLifecycle.Enable, CmdletNames.ISHExternalPreview, SupportsShouldProcess = false)]
    public sealed class EnableISHExternalPreviewCmdlet : BaseHistoryEntryCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        [Alias("extrId")]
        [ValidateNotNull]
        public string ExternalId { get; set; }

        protected override string HistoryEntry => $"{VerbsLifecycle.Enable}-{CmdletNames.ISHExternalPreview}"
                                                  + (!string.IsNullOrEmpty(ExternalId) ? $" -{ nameof(ExternalId)} ExternalId" : "");

        protected override string DeploymentSuffix => (ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment).Suffix;

        public override void ExecuteCmdlet()
        {
            var ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            var cmdSet = new EnableISHExternalPreviewCmdSet(Logger, ishPaths, ExternalId);

            cmdSet.Run();
        }
    }
}
