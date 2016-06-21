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
﻿using System;
using System.IO;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Extensions;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public abstract partial class BasePathsOperation
    {
        /// <summary>
        /// The instance of the deployment.
        /// </summary>
        private static Models.ISHDeployment _ishDeployment;

        /// <summary>
        /// The logger.
        /// </summary>
        protected static ILogger Logger;

        /// <summary>
        /// The instance of extended description of the deployment.
        /// </summary>
        private static Models.ISHDeploymentInternal _ishDeploymentInternal;
        /// <summary>
        /// <para type="description">Internal extended description of the instance of the Content Manager deployment.</para>
        /// </summary>
        public static Models.ISHDeploymentInternal ISHDeploymentInternal
        {
            get
            {
                if (_ishDeploymentInternal == null || _ishDeployment.Name != _ishDeploymentInternal.Name)
                {
                    var action = new GetISHDeploymentExtendedAction(Logger, _ishDeployment.Name,
                        result => _ishDeploymentInternal = result);
                    action.Execute();
                }
                return _ishDeploymentInternal;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BasePathsOperation"/> class.
        /// </summary>
        /// <param name="ishDeployment">The ish deployment.</param>
        /// <param name="logger"></param>
        public BasePathsOperation(ILogger logger, Models.ISHDeployment ishDeployment)
        {
            Logger = logger;
            _ishDeployment = ishDeployment;
        }

        /// <summary>
        /// The path to generated History.ps1 file
        /// </summary>
        protected static string HistoryFilePath => Path.Combine(ISHDeploymentInternal.GetDeploymentAppDataFolder(), "History.ps1");

        /// <summary>
        /// Converts the local folder path to UNC path.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns>Path to folder in UTC format</returns>
        private static string ConvertLocalFolderPathToUNCPath(string localPath)
        {
            return $@"\\{Environment.MachineName}\{localPath.Replace(":", "$")}";
        }
    }
}
