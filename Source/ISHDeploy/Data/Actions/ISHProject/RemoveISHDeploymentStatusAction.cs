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

using ISHDeploy.Common;
using ISHDeploy.Common.Interfaces.Actions;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Data.Actions.ISHProject
{
    /// <summary>
    /// Removes the status of deployment from Registry.
    /// </summary>
    /// <seealso cref="IRestorableAction" />
    public class RemoveISHDeploymentStatusAction : IAction
    {
        /// <summary>
        /// The data aggregate helper
        /// </summary>
        private ITrisoftRegistryManager _trisoftRegistryManager;

        /// <summary>
        /// The data aggregate helper
        /// </summary>
        private readonly string _projectName;


        /// <summary>
        /// Initializes new instance of the <see cref="RemoveISHDeploymentStatusAction"/>
        /// </summary>
        /// <param name="projectName">The deployment name</param>
        public RemoveISHDeploymentStatusAction(string projectName)
        {
            _trisoftRegistryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            _projectName = projectName;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public void Execute()
        {
            _trisoftRegistryManager.RemoveISHDeploymentStatus(_projectName);
        }
    }
}
