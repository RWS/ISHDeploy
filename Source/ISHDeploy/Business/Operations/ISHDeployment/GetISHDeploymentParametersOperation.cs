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
using ISHDeploy.Interfaces;
using ISHDeploy.Business.Invokers;
using System.Collections.Generic;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Gets parameters.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHDeploymentParametersOperation : BaseOperationPaths
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Parameters 
        /// </summary>
        private IEnumerable<ISHDeploymentParameter> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentParametersOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
        /// <param name="original">Flag to show initial parameters</param>
        /// <param name="changed">Flag to show difference in parameters only</param>
        /// <param name="showPassword">Flag to show real passwords</param>
        public GetISHDeploymentParametersOperation(ILogger logger, Models.ISHDeployment ishDeployment, bool original, bool changed, bool showPassword) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Getting parameters.");

            _invoker.AddAction(new GetISHDeploymentParametersAction (
                logger,
                InputParametersFilePath.VanillaPath,
                InputParametersFilePath.AbsolutePath,
                original, 
                changed, 
                showPassword, 
                result => _parameters = result));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>Collection with parameters for Content Manager deployments.</returns>
        public IEnumerable<ISHDeploymentParameter> Run()
        {
            _invoker.Invoke();

            return _parameters;
        }
    }
}
