using System.Linq;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.ISHProject;
using InfoShare.Deployment.Data.Actions.ISHProjectActions;
using InfoShare.Deployment.Extensions;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHDeployment
{
    public class UndoISHDeploymentOperation : IOperation
	{
		private readonly IActionInvoker _invoker;

		private Models.ISHDeployment _ishDeployment;

		/// <summary>
		/// Reverts all changes to the vanilla
		/// </summary>
		/// <param name="logger">Logger object</param>
		/// <param name="projectSuffix">Deployment project suffix</param>
        public UndoISHDeploymentOperation(ILogger logger, string projectSuffix)
        {
			_invoker = new ActionInvoker(logger, "InfoShare ExternalPreview deactivation");

			// Retrieveing deployment per suffix
			_invoker.AddAction(new GetISHDeploymentsAction(logger, projectSuffix, result => _ishDeployment = result.First()));

			// Rolling back changes for Web folder
			_invoker.AddAction(new UndoISHDeploymentsAction(logger, _ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Web), _ishDeployment.WebPath));
			
			// Rolling back changes for Data folder
			_invoker.AddAction(new UndoISHDeploymentsAction(logger, _ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Data), _ishDeployment.DataPath));

			// Rolling back changes for App folder
			_invoker.AddAction(new UndoISHDeploymentsAction(logger, _ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.App), _ishDeployment.AuthorFolderPath));

			// Removing licenses
			ISHFilePath licenseFolderPath = (new ISHPaths(_ishDeployment)).LicenceFolderPath;
			_invoker.AddAction(new UndoISHDeploymentsAction(logger, licenseFolderPath.AbsolutePath));
		}

		/// <summary>
		/// Runs commans invocation 
		/// </summary>
		public void Run()
        {
			_invoker.Invoke();
		}
    }
}
