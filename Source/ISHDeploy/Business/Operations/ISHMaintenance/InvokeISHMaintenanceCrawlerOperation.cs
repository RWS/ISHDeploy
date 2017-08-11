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
    /// Invokes operation of Crawler registration/unregister or reindex.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class InvokeISHMaintenanceCrawlerOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeISHMaintenanceCrawlerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="operationType">The operation type (register or unregister).</param>
        public InvokeISHMaintenanceCrawlerOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, RegisterCrawlerOperationType operationType) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"{operationType}ing the Crawler for '{CrawlerTridkApplicationName}'");

            Invoker.AddAction(new StartProcessAction(Logger, CrawlerExeFilePath, $"--{operationType} \"{CrawlerTridkApplicationName}\""));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeISHMaintenanceCrawlerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="reindexCardType">The reindex card type.</param>
        public InvokeISHMaintenanceCrawlerOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, string reindexCardType) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Registering the Crawler for '{CrawlerTridkApplicationName}'");

            if (string.IsNullOrEmpty(reindexCardType))
            {
                reindexCardType = "ISHAll";
            }
            
            Invoker.AddAction(new StartProcessAction(Logger, CrawlerExeFilePath, $"--{RegisterCrawlerOperationType.reindex} \"{CrawlerTridkApplicationName}\" {reindexCardType}"));
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
