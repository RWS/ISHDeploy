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
using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.COMPlus
{
    /// <summary>
    /// Start COM+ component.
    /// </summary>
    /// <seealso cref="IRestorableAction" />
    public class StartCOMPlusComponentAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The name of COM+ component.
        /// </summary>
        private readonly string _comPlusComponentName;

        /// <summary>
        /// The COM+ component manager
        /// </summary>
        private readonly ICOMPlusComponentManager _comPlusComponentManager;

        /// <summary>
        /// The COM+ component manager
        /// </summary>
        private bool _comPlusComponentWasStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartCOMPlusComponentAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        public StartCOMPlusComponentAction(ILogger logger, string comPlusComponentName)
            : base(logger)
        {
            _comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
            _comPlusComponentName = comPlusComponentName;
        }

        /// <summary>
        ///	Gets current value before change.
        /// </summary>
        public void Backup()
        {
            _comPlusComponentWasStarted = _comPlusComponentManager.IsComPlusComponentRunning(_comPlusComponentName, false);
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _comPlusComponentManager.StartCOMPlusComponents(_comPlusComponentName);

            if (!_comPlusComponentManager.IsComPlusComponentRunning(_comPlusComponentName))
            {
                throw new Exception($"COM+ component `{_comPlusComponentName}` has not been started");
            }
        }

        /// <summary>
        ///	Reverts a value to initial state.
        /// </summary>
        public void Rollback()
        {
            if (_comPlusComponentWasStarted)
            {
                _comPlusComponentManager.StartCOMPlusComponents(_comPlusComponentName);
            }
            else
            {
                _comPlusComponentManager.ShutdownCOMPlusComponents(_comPlusComponentName);
            }
        }
    }
}
