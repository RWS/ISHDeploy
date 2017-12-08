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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.COMPlus
{
    /// <summary>
    /// Sets credentials for COM+ component.
    /// </summary>
    /// <seealso cref="IRestorableAction" />
    public class SetCOMPlusCredentialsAction : BaseAction, IRestorableAction
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
        /// The windows service userName
        /// </summary>
        private readonly string _userName;

        /// <summary>
        /// The windows service previous UserName
        /// </summary>
        private readonly string _previousUserName;

        /// <summary>
        /// The windows service password
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// The windows service previous password
        /// </summary>
        private readonly string _previousPassword;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetCOMPlusCredentialsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="comPlusComponentName">The name of COM+ component.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="previousUserName">The previous user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="previousPassword">The previous password.</param>
        public SetCOMPlusCredentialsAction(ILogger logger, string comPlusComponentName, string userName, string previousUserName, string password, string previousPassword)
            : base(logger)
        {
            _comPlusComponentName = comPlusComponentName;

            _comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
            _userName = userName;
            _previousUserName = previousUserName;
            _password = password;
            _previousPassword = previousPassword;
        }

        /// <summary>
        ///	Gets current value before change.
        /// </summary>
        public void Backup()
        {
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _comPlusComponentManager.SetCOMPlusComponentCredentials(_comPlusComponentName, _userName, _password);
        }

        /// <summary>
        ///	Reverts a value to initial state.
        /// </summary>
        public void Rollback()
        {
            _comPlusComponentManager.SetCOMPlusComponentCredentials(_comPlusComponentName, _previousUserName, _previousPassword);
        }
    }
}
