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
using ISHDeploy.Data.Actions.ISHUIElement;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI;

namespace ISHDeploy.Business.Operations.ISHUIElement
{
    /// <summary>
    /// Inserts or updates the element of UI.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetUIElementOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUIElementOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="model">The model that represents UI element.</param>
        public SetUIElementOperation(ILogger logger, 
            Models.ISHDeployment ishDeployment,
            BaseUIElement model) :
            base(logger, ishDeployment)
        {
            var filePath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, model.RelativeFilePath);
            _invoker = new ActionInvoker(logger, $"Insert/Update `{model.XPath}` element in file {filePath.AbsolutePath}");

            _invoker.AddAction(new SetUIElementAction(
                logger,
                filePath,
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
