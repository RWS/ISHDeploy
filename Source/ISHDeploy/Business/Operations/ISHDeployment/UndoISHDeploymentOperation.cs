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
    public class UndoISHDeploymentOperation : OperationPaths, IOperation
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
		public UndoISHDeploymentOperation(ILogger logger, Models.ISHDeploymentExtended deployment)
		{
			Models.ISHDeploymentExtended ishDeployment = deployment;

			_invoker = new ActionInvoker(logger, "Reverting of changes to Vanilla state");

			// Rolling back changes for Web folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHFilePath.IshDeploymentType.Web), ishDeployment.GetAuthorFolderPath()));

			// Rolling back changes for Data folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHFilePath.IshDeploymentType.Data), ishDeployment.GetDataFolderPath()));
            
			// Rolling back changes for App folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHFilePath.IshDeploymentType.App), ishDeployment.GetAppFolderPath()));

			// Removing licenses
			_invoker.AddAction(new FileCleanDirectoryAction(logger, FoldersPaths.LicenceFolderPath.AbsolutePath));

			// Removing Backup folder
			_invoker.AddAction(new DirectoryRemoveAction(logger, deployment.GetDeploymentAppDataFolder()));

            // Cleaning up STS App_Data folder
            _invoker.AddAction(new FileCleanDirectoryAction(logger, deployment.WebNameSTSAppData));
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
