using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.File;
using InfoShare.Deployment.Data.Actions.ISHProject;
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

		private Models.ISHDeployment _ishDeployment;

		/// <summary>
		/// Reverts all changes to the vanilla
		/// </summary>
		/// <param name="logger">Logger object</param>
		/// <param name="projectSuffix">Deployment project suffix</param>
        public UndoISHDeploymentOperation(ILogger logger, string projectSuffix)
        {
			_invoker = new ActionInvoker(logger, "InfoShare ExternalPreview deactivation");

			// Retrieving deployment per suffix

			// (!)	We need to retrieve deployment before accessing next action
			(new GetISHDeploymentsAction(logger, projectSuffix, result => _ishDeployment = result.First())).Execute();

			// Rolling back changes for Web folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, _ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Web), _ishDeployment.WebPath));

			// Rolling back changes for Data folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, _ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.Data), _ishDeployment.DataPath));

			// Rolling back changes for App folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, _ishDeployment.GetDeploymentTypeBackupFolder(ISHPaths.IshDeploymentType.App), _ishDeployment.AuthorFolderPath));

			// Removing licenses
			ISHFilePath licenseFolderPath = (new ISHPaths(_ishDeployment)).LicenceFolderPath;
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
