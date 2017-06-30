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
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.Process;

namespace ISHDeploy.Business.Operations.ISHMaintenance
{
    /// <summary>
    /// Invokes operation of Crawler registration.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class InvokeISHMaintenanceRegisterCrawlerOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeISHMaintenanceRegisterCrawlerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="operationType">The operation type (register or unregister).</param>
        /// <param name="crawlerTridkApp">The TridkApp value to registrate the Crawler for it.</param>
        public InvokeISHMaintenanceRegisterCrawlerOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, RegisterCrawlerOperationType operationType, string crawlerTridkApp) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Registering the Crawler for '{crawlerTridkApp}'");

            if (string.IsNullOrEmpty(crawlerTridkApp))
            {
                crawlerTridkApp = "InfoShareBuilders";
            }

            Invoker.AddAction(new StartProcessAction(Logger, CrawlerExeFilePath, $"--{operationType} \"{crawlerTridkApp}\""));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }
    }
}
