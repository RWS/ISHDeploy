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
﻿using System.IO;
using ISHDeploy.Extensions;

namespace ISHDeploy.Models
{
    /// <summary>
    /// Wrapper for file path that provides relative, absolute and vanilla installation path of the file.
    /// </summary>
    public class ISHFilePath
	{
        /// <summary>
        /// Specifies Content Manager main folders
        /// </summary>
		public enum IshDeploymentType
        {
            /// <summary>
            /// Content Manager Web folder
            /// </summary>
			Web,
            /// <summary>
            /// Content Manager Data folder
            /// </summary>
			Data,
            /// <summary>
            /// Content Manager App folder
            /// </summary>
			App
        }

        /// <summary>
        /// Absolute path to file or folder
        /// </summary>
        public string AbsolutePath { get; }

        /// <summary>
        /// Gets the relative path of the file.
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// Gets the vanilla installation path.
        /// </summary>
        public string VanillaPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHFilePath"/> class.
        /// </summary>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="deploymentType">Type of the deployment.</param>
        /// <param name="relativePath">The relative path to the file.</param>
        public ISHFilePath(ISHDeploymentInternal ishDeployment, IshDeploymentType deploymentType, string relativePath)
		{
            if (deploymentType == IshDeploymentType.App)
            {
                AbsolutePath = Path.Combine(ishDeployment.AppFolderPath, relativePath);
            }
            else if (deploymentType == IshDeploymentType.Web)
            {
                AbsolutePath = Path.Combine(ishDeployment.AuthorFolderPath, relativePath);
            }
            else if (deploymentType == IshDeploymentType.Data)
            {
                AbsolutePath = Path.Combine(ishDeployment.DataFolderPath, relativePath);
            }

			RelativePath = relativePath;
            VanillaPath = Path.Combine(ishDeployment.GetDeploymentTypeBackupFolder(deploymentType), RelativePath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHFilePath"/> class.
        /// </summary>
        /// <param name="absolutePath">The path to file in deployment.</param>
        /// <param name="backupPath">The path to backup file.</param>
        public ISHFilePath(string absolutePath, string backupPath)
        {
            AbsolutePath = absolutePath;
            VanillaPath = backupPath;
        }
	}
}
