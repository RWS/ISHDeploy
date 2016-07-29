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
﻿using ISHDeploy.Interfaces;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Gets history file content.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHDeploymentHistoryOperation : BaseOperationPaths, IOperation<string>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The history file content
        /// </summary>
        private string _historyContent = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentsOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
        public GetISHDeploymentHistoryOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Getting the history file content");

            _invoker.AddAction(new FileReadAllTextAction(logger, HistoryFilePath, result => _historyContent = result));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public string Run()
        {
            _invoker.Invoke();

            return _historyContent;
        }
    }
}
