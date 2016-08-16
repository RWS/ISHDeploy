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
﻿using System.Collections.Generic;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Interfaces;
using ISHDeploy.Business.Invokers;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Gets a list of installed Content Manager deployments found on the current system.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHDeploymentsOperation : IOperation<IEnumerable<Models.ISHDeployment>>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The list of installed Content Manager deployments found on the current system.
        /// </summary>
        private IEnumerable<Models.ISHDeployment> _ishProjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentsOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="projectName">The deployment name.</param>
        public GetISHDeploymentsOperation(ILogger logger, string projectName)
        {
            _invoker = new ActionInvoker(logger, "Get a list of installed Content Manager deployments");
            _invoker.AddAction(new GetISHDeploymentsAction(logger, projectName, result => _ishProjects = result));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public IEnumerable<Models.ISHDeployment> Run()
        {
            _invoker.Invoke();

            return _ishProjects;
        }
    }
}
