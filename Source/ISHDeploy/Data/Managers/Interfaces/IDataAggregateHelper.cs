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

using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.Backup;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Aggregates data from different places.
    /// </summary>
    public interface IDataAggregateHelper
    {
        /// <summary>
        /// Returns path to ISHDeployment module ProgramData folder (such as "C:\ProgramData\ISHDeploy.12.x.x\InfoShareXXX")
        /// </summary>
        /// <param name="deploymentName">The Content Manager deployment name.</param>
        /// <returns>The path to special folder where the module store temp data and original (vanilla) properties of specified environment</returns>
        string GetPathToISHDeploymentProgramDataFolder(string deploymentName);

        /// <summary>
        /// Returns input parameters
        /// </summary>
        /// <param name="deploymentName">The Content Manager deployment name.</param>
        /// <returns>InputParameters containing all parameters from InputParameters.xml file for specified deployment</returns>
        InputParameters GetInputParameters(string deploymentName);

        /// <summary>
        /// Return actual state of components
        /// </summary>
        /// <param name="filePath">The path to file with saved states of all components of deployment.</param> 
        /// <param name="deploymentName">The Content Manager deployment name.</param>
        /// <returns>The collection of components for specified deployment with their actual state</returns>
        ISHComponentsCollection GetActualStateOfComponents(string filePath, string deploymentName);

        /// <summary>
        /// Save all components of deployment
        /// </summary>
        /// <param name="filePath">The path to file.</param>
        /// <param name="collection">The collection of components for specified deployment</param>
        void SaveComponents(string filePath, ISHComponentsCollection collection);

        /// <summary>
        /// Return the state of the components that should be in the system now
        /// </summary>
        /// <param name="filePath">The path to file with saved states of all components of deployment.</param>
        /// <returns>The collection of components with their states. If the file with saved states does not exist then return the default (vanilla) state.</returns>
        ISHComponentsCollection GetExpectedStateOfComponents(string filePath);

        /// <summary>
        /// Returns all windows services with all properties needed for their recreation
        /// </summary>
        /// <param name="deploymentName">The name of deployment.</param>
        /// <returns>The collection of windows services with all properties needed for their recreation</returns>
        ISHWindowsServiceBackupCollection GetISHWindowsServiceBackupCollection(string deploymentName);

        /// <summary>
        /// Gets the status of deployment.
        /// </summary>
        /// <param name="deploymentName">The name of deployment.</param>
        /// <returns>Status of deployments</returns>
        ISHDeploymentStatus GetISHDeploymentStatus(string deploymentName);

        /// <summary>
        /// Saves the status of deployment.
        /// </summary>
        /// <param name="deploymentName">The name of deployment.</param>
        /// <param name="status">The status of deployment.</param>
        void SaveISHDeploymentStatus(string deploymentName, ISHDeploymentStatus status);
    }
}
