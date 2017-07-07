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

using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Business.Operations.ISHMaintenance
{
    /// <summary>
    /// Invokes operation of SolrLucene clean up.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class InvokeISHMaintenanceCleanUpSolrLuceneOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeISHMaintenanceCleanUpSolrLuceneOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public InvokeISHMaintenanceCleanUpSolrLuceneOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "SolrLucene cleaning up");

            // Stop Crawler and SolrLucene services
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.Crawler, ISHWindowsServiceType.SolrLucene).Where(s => s.Status == ISHWindowsServiceStatus.Running);
            foreach (var service in services)
            {
                Invoker.AddAction(new StopWindowsServiceAction(Logger, service));
            }

            // Clean up folders
            Invoker.AddAction(new FileCleanDirectoryAction(logger, SolrLuceneCatalogAllVersionsFolderPath));
            Invoker.AddAction(new FileCleanDirectoryAction(logger, SolrLuceneCatalogLatestVersionFolderPath));
            Invoker.AddAction(new FileCleanDirectoryAction(logger, SolrLuceneCatalogCrawlerFileCacheAllVersionsFolderPath));
            Invoker.AddAction(new FileCleanDirectoryAction(logger, SolrLuceneCatalogCrawlerFileCacheLatestVersionFolderPath));
            Invoker.AddAction(new FileCleanDirectoryAction(logger, SolrLuceneCatalogCrawlerFileCacheISHReusableObjectFolderPath));

            // Start Crawler and SolrLucene services
            foreach (var service in services)
            {
                Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
            }
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
