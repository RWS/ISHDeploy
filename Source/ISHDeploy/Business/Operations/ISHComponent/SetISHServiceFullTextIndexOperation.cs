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
using ISHDeploy.Common.Models.Backup;
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

            // Change RegistryValue and do vanilla backup, if vanilla value has not been saved yet
            Invoker.AddAction(new SetRegistryValueAction(logger, new RegistryValue { Key = RegInfoShareAuthorRegistryElement, ValueName = RegistryValueName.SolrLuceneBaseUrl, Value = uri }, VanillaRegistryValuesFilePath));
            Invoker.AddAction(new SetRegistryValueAction(logger, new RegistryValue { Key = RegInfoShareBuildersRegistryElement, ValueName = RegistryValueName.SolrLuceneBaseUrl, Value = uri }, VanillaRegistryValuesFilePath));

            // Remove dependencies between Crawler and SolrLucene
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.Crawler);

            foreach (var service in services)
            {
                Invoker.AddAction(new SetRegistryValueAction(logger, string.Format(RegWindowsServicesRegistryPathPattern, service.Name), RegistryValueName.DependOnService, string.Empty));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceFullTextIndexOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="port">The target lucene port.</param>
        /// <param name="portRegistryValueName">The name of port value in registry.</param>
        /// <param name="solrLuceneStopKey">The StopKey.</param>
        public SetISHServiceFullTextIndexOperation(ILogger logger, Models.ISHDeployment ishDeployment, int port, RegistryValueName portRegistryValueName, string solrLuceneStopKey = "") :
            base(logger, ishDeployment)
        {

            if (portRegistryValueName != RegistryValueName.SolrLuceneServicePort ||
                portRegistryValueName != RegistryValueName.SolrLuceneStopPort)
            {
                throw new AggregateException($"Operation does not support such RegistryValueName value: {portRegistryValueName}");
            }

            Invoker = new ActionInvoker(logger, $"Setting of target lucene {portRegistryValueName} port of {ISHWindowsServiceType.SolrLucene} windows services");

            // Make sure Vanilla backup of all windows services exists
            Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

            Invoker.AddAction(new SetRegistryValueAction(logger, RegInfoShareBuildersRegistryElement, portRegistryValueName, port));

            if (portRegistryValueName == RegistryValueName.SolrLuceneServicePort)
            {
                var registryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
                var currentLuceneUri =
                    new Uri(
                        registryManager.GetRegistryValue(RegInfoShareBuildersRegistryElement,
                            RegistryValueName.SolrLuceneBaseUrl).ToString());

                var newUriAsString = currentLuceneUri.ToString().Replace(currentLuceneUri.Port.ToString(), port.ToString());
                var newUri = new Uri(newUriAsString);

                Invoker.AddAction(new SetRegistryValueAction(logger, RegInfoShareAuthorRegistryElement, RegistryValueName.SolrLuceneBaseUrl, newUri));
                Invoker.AddAction(new SetRegistryValueAction(logger, RegInfoShareBuildersRegistryElement, RegistryValueName.SolrLuceneBaseUrl, newUri));
            }
            else
            {
                Invoker.AddAction(new SetRegistryValueAction(logger, RegInfoShareAuthorRegistryElement, RegistryValueName.SolrLuceneStopKey, solrLuceneStopKey));
            }

            // Open port

            // Remove dependencies between Crawler and SolrLucene
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.Crawler);

            foreach (var service in services)
            {
                // Change RegistryValue for Crawler services.
                // There is no need to backup the vanilla value of the registry, since the service will be completely re-created in case of Undo
                Invoker.AddAction(new SetRegistryValueAction(logger, new RegistryValue { Key = string.Format(RegWindowsServicesRegistryPathPattern, service.Name), ValueName = RegistryValueName.DependOnService, Value = string.Empty }));
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
