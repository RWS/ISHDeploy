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
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces.Actions;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Data.Actions.ISHProject
{

    /// <summary>
    /// Saves the status of deployment.
    /// </summary>
    /// <seealso cref="IRestorableAction" />
    public class SaveISHDeploymentStatusAction : IAction, IRestorableAction
    {
        /// <summary>
        /// The data aggregate helper
        /// </summary>
        private readonly IDataAggregateHelper _dataAggregateHelper;

        /// <summary>
        /// The data aggregate helper
        /// </summary>
        private readonly string _projectName;

        /// <summary>
        /// The previous status of deployment
        /// </summary>
        private ISHDeploymentStatus _previousStatus;

        /// <summary>
        /// The new status of deployment
        /// </summary>
        private readonly ISHDeploymentStatus _newStatus;

        /// <summary>
        /// Initializes new instance of the <see cref="SaveISHDeploymentStatusAction"/>
        /// </summary>
        /// <param name="projectName">The deployment name</param>
        /// <param name="status">The deployment status</param>
        public SaveISHDeploymentStatusAction(string projectName, ISHDeploymentStatus status)
        {
            _dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            _projectName = projectName;
            _newStatus = status;
        }

        /// <summary>
        ///	Creates backup of the asset.
        /// </summary>
        public void Backup()
        {
            _previousStatus = _dataAggregateHelper.GetISHDeploymentStatus(_projectName);
        }

        /// <summary>
        ///	Reverts an asset to initial state.
        /// </summary>
        public void Rollback()
        {
            _dataAggregateHelper.SaveISHDeploymentStatus(_projectName, _previousStatus);
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public void Execute()
        {
            _dataAggregateHelper.SaveISHDeploymentStatus(_projectName, _newStatus);
        }
    }
}
