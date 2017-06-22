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
    /// Opens port.
    /// </summary>
    /// <seealso cref="IRestorableAction" />
    public class OpenPortAction : BaseAction
    {
        /// <summary>
        /// The COM+ component manager
        /// </summary>
        private readonly ICOMPlusComponentManager _comPlusComponentManager;

        /// <summary>
        /// The port number
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenPortAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="port">The port.</param>
        public OpenPortAction(ILogger logger, int port)
            : base(logger)
        {
            _port = port;
            _comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _comPlusComponentManager.GloballyOpenPort(_port);
        }
    }
}
