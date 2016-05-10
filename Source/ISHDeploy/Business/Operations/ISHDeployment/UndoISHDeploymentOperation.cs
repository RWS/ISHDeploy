using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Extensions;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
	/// <summary>
	/// Operation to revert changes to Vanilla state
	/// </summary>
    public class UndoISHDeploymentOperation : IOperation
	{
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

		/// <summary>
		/// Reverts all changes to the vanilla
		/// </summary>
		/// <param name="logger">Logger object.</param>
		/// <param name="deployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
		public UndoISHDeploymentOperation(ILogger logger, Models.ISHDeployment deployment)
		{
			Models.ISHDeployment ishDeployment = deployment;

			_invoker = new ActionInvoker(logger, "Reverting of changes to Vanilla state");

			// Rolling back changes for Web folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Web), ishDeployment.GetAuthorFolderPath()));

			// Rolling back changes for Data folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Data), ishDeployment.GetDataFolderPath()));
            
			// Rolling back changes for App folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.App), ishDeployment.GetAppFolderPath()));

			// Removing licenses
			ISHFilePath licenseFolderPath = new ISHPaths(ishDeployment).LicenceFolderPath;
			_invoker.AddAction(new FileCleanDirectoryAction(logger, licenseFolderPath.AbsolutePath));

			// Removing Backup folder
			_invoker.AddAction(new DirectoryRemoveAction(logger, deployment.GetDeploymentAppDataFolder()));
		}

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
			_invoker.Invoke();
		}
	}
}
