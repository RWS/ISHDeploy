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

using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.Registry;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Sets amount of ISH windows service.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHServiceFullTextIndexOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceFullTextIndexOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="uri">The target lucene Uri.</param>
        public SetISHServiceFullTextIndexOperation(ILogger logger, Models.ISHDeployment ishDeployment, Uri uri) :
            base(logger, ishDeployment)
        {

            Invoker = new ActionInvoker(logger, $"Setting of target lucene Uri of {ISHWindowsServiceType.SolrLucene} windows services");

            // Make sure Vanilla backup of all windows services exists
            Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

            Invoker.AddAction(new SetRegistryValueAction(logger, RegInfoShareAuthorRegistryElement, RegistryValueName.SolrLuceneBaseUrl, uri));
            Invoker.AddAction(new SetRegistryValueAction(logger, RegInfoShareBuildersRegistryElement, RegistryValueName.SolrLuceneBaseUrl, uri));

            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.Crawler);

            foreach (var service in services)
            {
                Invoker.AddAction(new SetRegistryValueAction(logger, string.Format(RegWindowsServicesRegistryPathPattern, service.Name), RegistryValueName.DependOnService, string.Empty));
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
