using System.Management.Automation;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Set, "ISHDeployment", SupportsShouldProcess = false)]
    public sealed class SetISHDeploymentCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public Models.ISHDeployment IshDeployment { get; set; }

        public override void ExecuteCmdlet()
        {
            ISHProjectProvider.Instance.InitializeIshProject(IshDeployment);
        }
    }
}
