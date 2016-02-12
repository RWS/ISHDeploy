using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHExternalPreview
{
    [Cmdlet(VerbsLifecycle.Enable, "ISHExternalPreview", SupportsShouldProcess = false)]
    public sealed class EnableISHExternalPreviewCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNullOrEmpty]
        public Models.ISHDeployment ISHDeployment { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        [Alias("extrId")]
        [ValidateNotNullOrEmpty]
        public string ExternalId { get; set; }

        public override void ExecuteCmdlet()
        {
            var ishDeployment = ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment;
            var cmdSet = new EnableISHExternalPreviewCmdSet(this, ishDeployment.AuthorFolderPath, ExternalId);

            cmdSet.Run();
        }
    }
}
