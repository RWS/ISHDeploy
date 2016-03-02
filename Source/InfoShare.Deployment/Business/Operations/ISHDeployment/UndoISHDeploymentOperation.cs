using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.File;
using InfoShare.Deployment.Extensions;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.Operations.ISHDeployment
{
	/// <summary>
	/// Operation to revert changes to Vanilla state
	/// </summary>
    public class UndoISHDeploymentOperation : IOperation
	{
		private readonly IActionInvoker _invoker;

		/// <summary>
		/// Reverts all changes to the vanilla
		/// </summary>
		/// <param name="logger">Logger object</param>
		/// <param name="deployment">Ish deployment instance <see cref="T:InfoShare.Deployment.Models.ISHDeployment"/></param>
		public UndoISHDeploymentOperation(ILogger logger, Models.ISHDeployment deployment)
		{
			Models.ISHDeployment ishDeployment = deployment;

			_invoker = new ActionInvoker(logger, "InfoShare ExternalPreview deactivation");

			// Rolling back changes for Web folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Web), ishDeployment.WebPath));

			// Rolling back changes for Data folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Data), ishDeployment.DataPath));

			// Rolling back changes for App folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.App), ishDeployment.AuthorFolderPath));

			// Removing licenses
			ISHFilePath licenseFolderPath = (new ISHPaths(ishDeployment)).LicenceFolderPath;
			_invoker.AddAction(new FileCleanDirectoryAction(logger, licenseFolderPath.AbsolutePath));
		}

		/// <summary>
		/// Runs commands invocation 
		/// </summary>
		public void Run()
        {
			_invoker.Invoke();
		}
	}
}
