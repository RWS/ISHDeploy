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
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Gets parameters.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHComponentOperation : BaseOperationPaths, IOperation<ISHComponentsCollection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
        public GetISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>Collection with parameters for Content Manager deployments.</returns>
        public ISHComponentsCollection Run()
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            if (!fileManager.FileExists(CurrentISHComponentStatesFilePath.AbsolutePath))
            {
                return new ISHComponentsCollection(true);
            }
            else
            {
                return dataAggregateHelper.ReadComponentsFromFile(CurrentISHComponentStatesFilePath.AbsolutePath);
            }
        }
    }
}
