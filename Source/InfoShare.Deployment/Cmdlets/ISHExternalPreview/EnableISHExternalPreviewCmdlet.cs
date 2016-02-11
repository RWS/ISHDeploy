using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Models;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHExternalPreview
{
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class EnableISHExternalPreviewCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNullOrEmpty]
        public ISHProject IshProject { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        [Alias("proj")]
        [ValidateNotNullOrEmpty]
        public string ExternalId { get; set; }

        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new EnableISHExternalPreviewCmdSet(this, IshProject ?? ISHProjectProvider.Instance.IshProject, ExternalId);

            cmdSet.Run();
        }
    }
}
