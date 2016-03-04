using System.Management.Automation;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Business.Operations.ISHUITranslationJob;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHUITranslationJob
{
    [Cmdlet(VerbsLifecycle.Enable, "ISHUITranslationJob", SupportsShouldProcess = false)]
    public class EnableISHUITranslationJobCmdlet : BaseHistoryEntryCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        private ISHPaths _ishPaths;
        protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment));

        public override void ExecuteCmdlet()
        {
            var operation = new EnableISHUITranslationJobOperation(Logger, IshPaths);

            operation.Run();
        }
    }
}
