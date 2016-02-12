using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHUIQualityAssistant;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHUIQualityAssistant
{
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIQualityAssistant", SupportsShouldProcess = false)]
    public sealed class DisableISHUIQualityAssistantCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNullOrEmpty]
        public Models.ISHDeployment ISHDeployment { get; set; }

        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new DisableISHUIQualityAssistantCmdSet(this, ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment);

            cmdSet.Run();
        }
    }
}
