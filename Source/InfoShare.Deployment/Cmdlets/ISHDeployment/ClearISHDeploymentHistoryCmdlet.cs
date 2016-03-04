using System.Management.Automation;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Clear, "ISHDeploymentHistory")]
    public class ClearISHDeploymentHistoryCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        private ISHPaths _ishPaths;
        protected ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment));

        public override void ExecuteCmdlet()
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();

	        var historyFilePath = IshPaths.HistoryFilePath;
	        if (fileManager.Exists(historyFilePath))
	        {
		        fileManager.Delete(IshPaths.HistoryFilePath);
	        }
        }
    }
}
