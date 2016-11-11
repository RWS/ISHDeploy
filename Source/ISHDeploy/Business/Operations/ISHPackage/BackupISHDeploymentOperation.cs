/*
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
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Cmdlets.ISHPackage;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// Backup data from Web, App, Data folders.
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class BackupISHDeploymentOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyISHCMFileOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="parameterSetName">Web, App or Data folders.</param>
        /// <param name="path">Path to backup.</param>
        public BackupISHDeploymentOperation(ILogger logger, Models.ISHDeployment ishDeployment, string parameterSetName, string[] path) :
            base(logger, ishDeployment)
        {
            string sourceFolderPath, destinationFolderPath;

            switch (parameterSetName)
            {
                case "Web":
                    sourceFolderPath = AuthorFolderPath;
                    destinationFolderPath = BackupWebFolderPath;
                    break;
                case "App":
                    sourceFolderPath = AppFolderPath;
                    destinationFolderPath = BackupAppFolderPath;
                    break;
                case "Data":
                    sourceFolderPath = DataFolderPath;
                    destinationFolderPath = BackupDataFolderPath;
                    break;
                default:
                    throw new ArgumentException($"Folder for {nameof(BackupISHDeploymentCmdlet)} should be defined.");
            }

            foreach (var template in path)
            {
                _invoker = new ActionInvoker(logger, $"Backup {sourceFolderPath}\\{template} files.");
                _invoker.AddAction(new BackupAction(logger, sourceFolderPath, destinationFolderPath, template));
            }
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