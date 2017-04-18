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
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Actions.XmlFile;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHUIElement
{
    /// <summary>
    /// Remove UI element.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class RemoveUIElementOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveUIElementOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="model">The model that represents UI element.</param>
        public RemoveUIElementOperation(ILogger logger,
            Models.ISHDeployment ishDeployment,
            BaseXMLElement model) :
            base(logger, ishDeployment)
        {
            var filePath = new ISHFilePath(WebFolderPath, BackupWebFolderPath, model.RelativeFilePath);
            _invoker = new ActionInvoker(logger, $"Remove `{model.XPath}` element in file {filePath.AbsolutePath}");
            
            _invoker.AddAction(new RemoveElementAction(
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
