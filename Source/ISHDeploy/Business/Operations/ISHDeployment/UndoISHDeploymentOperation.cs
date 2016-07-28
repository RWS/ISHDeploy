/**
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Extensions;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
	/// <summary>
	/// Operation to revert changes to Vanilla state
	/// </summary>
    public class UndoISHDeploymentOperation : BasePathsOperation, IOperation
	{
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Gets or sets a value indicating whether skip recycle or not. For integration test perspective only.
        /// Please, see https://jira.sdl.com/browse/TS-11329
        /// </summary>
        /// <value>
        ///   <c>true</c> if [true] then skip recycle; otherwise do recycle</c>.
        /// </value>
        public static bool SkipRecycle { get; set; } = false;

        /// <summary>
        /// Reverts all changes to the vanilla
        /// </summary>
        /// <param name="logger">Logger object.</param>
        /// <param name="ishDeployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
        public UndoISHDeploymentOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
		{
            _invoker = new ActionInvoker(logger, "Reverting of changes to Vanilla state");

            if (!SkipRecycle)
            {
                // Stop Application pools before undo
                _invoker.AddAction(new StopApplicationPoolAction(logger, ISHDeploymentInternal.WSAppPoolName));
                _invoker.AddAction(new StopApplicationPoolAction(logger, ISHDeploymentInternal.STSAppPoolName));
                _invoker.AddAction(new StopApplicationPoolAction(logger, ISHDeploymentInternal.CMAppPoolName));
            }

            // Rolling back changes for Web folder
            _invoker.AddAction(new FileCopyDirectoryAction(logger, ISHDeploymentInternal.GetDeploymentTypeBackupFolder(ISHFilePath.IshDeploymentType.Web), ISHDeploymentInternal.AuthorFolderPath));

			// Rolling back changes for Data folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ISHDeploymentInternal.GetDeploymentTypeBackupFolder(ISHFilePath.IshDeploymentType.Data), ISHDeploymentInternal.DataFolderPath));
            
			// Rolling back changes for App folder
			_invoker.AddAction(new FileCopyDirectoryAction(logger, ISHDeploymentInternal.GetDeploymentTypeBackupFolder(ISHFilePath.IshDeploymentType.App), ISHDeploymentInternal.AppFolderPath));

            // Removing licenses
            _invoker.AddAction(new FileCleanDirectoryAction(logger, FoldersPaths.LicenceFolderPath.AbsolutePath));

            // Cleaning up STS App_Data folder
            _invoker.AddAction(new FileCleanDirectoryAction(logger, ISHDeploymentInternal.WebNameSTSAppData));

            // Restore InputParameters.xml
            bool isInputParameterBackupFileExist = false;
            (new FileExistsAction(logger, InputParameters.Path.VanillaPath, returnResult => isInputParameterBackupFileExist = returnResult)).Execute();
            if (isInputParameterBackupFileExist)
            {
                _invoker.AddAction(new FileCopyAction(logger, InputParameters.Path.VanillaPath, InputParameters.Path,
                    true));
            }

            if (!SkipRecycle)
            {
                // Recycling Application pools after undo
                _invoker.AddAction(new RecycleApplicationPoolAction(logger, ISHDeploymentInternal.WSAppPoolName, true));
                _invoker.AddAction(new RecycleApplicationPoolAction(logger, ISHDeploymentInternal.STSAppPoolName, true));
                _invoker.AddAction(new RecycleApplicationPoolAction(logger, ISHDeploymentInternal.CMAppPoolName, true));

                // Waiting until files becomes unlocked
                _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareAuthorWebConfig.Path));
                _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareSTSWebConfig.Path));
                _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareWSWebConfig.Path));
            }

			// Removing Backup folder
			_invoker.AddAction(new DirectoryRemoveAction(logger, ISHDeploymentInternal.GetDeploymentAppDataFolder()));
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
