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

using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.ISHUIAction;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI;

namespace ISHDeploy.Business.Operations.ISHUIOperation
{
    /// <summary>
    /// Disables Content Editor for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetUIOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUIOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public SetUIOperation(ILogger logger, 
            Models.ISHDeployment ishDeployment,
            BaseUIModel model) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Insert/Update UI/XML element");

            _invoker.AddAction(new SetUIAction(
                logger,
                new ISHFilePath(AuthorFolderPath, BackupWebFolderPath, model.RelativeFilePath),
                model));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
