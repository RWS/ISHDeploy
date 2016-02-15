using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHUIQualityAssistant;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHUIQualityAssistant
{
    [Cmdlet(VerbsLifecycle.Enable, "ISHUIQualityAssistant", SupportsShouldProcess = false)]
    public sealed class EnableISHUIQualityAssistantCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        public override void ExecuteCmdlet()
        {
            var ishDeployment = ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment;
            var cmdSet = new EnableISHUIQualityAssistantCmdSet(this, ishDeployment.AuthorFolderPath);

			cmdSet.Run();
        }
    }
}