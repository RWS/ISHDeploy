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
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHDeployment
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
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupISHDeploymentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="parameterSetName">Web, App or Data folders.</param>
        /// <param name="path">Path to backup.</param>
        public BackupISHDeploymentOperation(ILogger logger, Models.ISHDeployment ishDeployment, string parameterSetName, string[] path) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Backup files.");
            string sourceFolderPath, destinationFolderPath;

            switch (parameterSetName)
            {
                case "Web":
                    sourceFolderPath = WebFolderPath;
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
                    throw new ArgumentException($"Folder for {nameof(BackupISHDeploymentOperation)} should be defined.");
            }

            foreach (var template in path)
            {
                Invoker.AddAction(new BackupAction(logger, sourceFolderPath, destinationFolderPath, template));
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }
    }
}