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
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.COMPlus;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Start components of deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class StartISHComponentOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="componentsNames">Names of components to be enabled.</param>
        public StartISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment,
            params ISHComponentName[] componentsNames) :
                base(logger, ishDeployment)
        {
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            var components =
               dataAggregateHelper.GetExpectedStateOfComponents(CurrentISHComponentStatesFilePath.AbsolutePath).Components.Where(x => componentsNames.Contains(x.Name)).ToList();

            Invoker = new ActionInvoker(logger, "Enabling of components");

            InitializeActions(logger, ishDeployment, components);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">Array with the components to be Startd.</param>
        public StartISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, IEnumerable<Models.ISHComponent> components) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Enabling of components");

            InitializeActions(logger, ishDeployment, components);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="backgroundTaskRole">The role of BackgroundTask component to be started.</param>
        public StartISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, string backgroundTaskRole) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Enabling of BackgroundTask component with role `{backgroundTaskRole}`");
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            var componentsCollection =
               dataAggregateHelper.GetExpectedStateOfComponents(CurrentISHComponentStatesFilePath.AbsolutePath);

            var component = componentsCollection[ISHComponentName.BackgroundTask, backgroundTaskRole];

            if (component != null)
            {
                if (ishDeployment.Status == ISHDeploymentStatus.Started || ishDeployment.Status == ISHDeploymentStatus.Starting)
                {
                    var services = serviceManager.GetISHBackgroundTaskWindowsServices(ishDeployment.Name);
                    foreach (
                        var service in
                            services.Where(
                                x => string.Equals(x.Role, component.Role, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
                    }
                }
                else
                {
                    Logger.WriteWarning($"The background task with role '{backgroundTaskRole}' will not be started, because the deployment '{ishDeployment.Name}' is not started");
                }
            }
            else
            {
                throw new ArgumentException($"The BackgroundTask component with role `{backgroundTaskRole}` does not exist");
            }
        }

        /// <summary>
        /// Initializes actions of the <see cref="StartISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">Array with the components to be Startd.</param>
        private void InitializeActions(ILogger logger, Models.ISHDeployment ishDeployment,
            IEnumerable<Models.ISHComponent> components)
        {
            if (ishDeployment.Status == ISHDeploymentStatus.Started || ishDeployment.Status == ISHDeploymentStatus.Starting)
            {
                var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

                // Reorder Components Collection
                var orderedComponentsCollection = new List<Models.ISHComponent>();

                if (components.Any(x => x.Name == ISHComponentName.SolrLucene))
                {
                    // IF SolrLucene is present, make sure to start it before starting the Crawler
                    orderedComponentsCollection.Add(components.First(x => x.Name == ISHComponentName.SolrLucene));
                }

                orderedComponentsCollection.AddRange(components.Where(x => x.Name != ISHComponentName.SolrLucene && x.Name != ISHComponentName.COMPlus));

                if (components.Any(x => x.Name == ISHComponentName.COMPlus))
                {
                    // IF COM+ applications are present, make sure to start the IIS pools first.
                    orderedComponentsCollection.Add(components.First(x => x.Name == ISHComponentName.COMPlus));
                }

                foreach (var component in orderedComponentsCollection)
                {
                    IEnumerable<Models.ISHWindowsService> services;

                    switch (component.Name)
                    {
                        case ISHComponentName.CM:
                            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.CMAppPoolName,
                                true));
                            break;
                        case ISHComponentName.WS:
                            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.WSAppPoolName,
                                true));
                            break;
                        case ISHComponentName.STS:
                            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName,
                                true));
                            break;
                        case ISHComponentName.TranslationBuilder:
                            services = serviceManager.GetServices(ishDeployment.Name,
                                ISHWindowsServiceType.TranslationBuilder);
                            foreach (var service in services)
                            {
                                Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
                            }
                            break;
                        case ISHComponentName.TranslationOrganizer:
                            services = serviceManager.GetServices(ishDeployment.Name,
                                ISHWindowsServiceType.TranslationOrganizer);
                            foreach (var service in services)
                            {
                                Invoker.AddAction(
                                    new StartWindowsServiceAction(Logger, service));
                            }
                            break;
                        case ISHComponentName.COMPlus:
                            Invoker.AddAction(
                                new StartCOMPlusComponentAction(Logger, TrisoftInfoShareAuthorComPlusApplicationName));
                            break;
                        case ISHComponentName.Crawler:
                            services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.Crawler);
                            foreach (var service in services)
                            {
                                Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
                            }
                            break;
                        case ISHComponentName.SolrLucene:
                            services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.SolrLucene);
                            foreach (var service in services)
                            {
                                Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
                            }
                            break;
                        case ISHComponentName.BackgroundTask:
                            var backgroundTaskServices = serviceManager.GetISHBackgroundTaskWindowsServices(ishDeployment.Name);
                            foreach (var backgroundTaskService in backgroundTaskServices)
                            {
                                if (backgroundTaskService.Role.Equals(component.Role, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Invoker.AddAction(new StartWindowsServiceAction(Logger, backgroundTaskService));
                                }
                            }
                            break;
                        default:
                            Logger.WriteDebug($"Unsupported component type: {component.Name}");
                            break;
                    }
                }
            }
            else
            {
                Logger.WriteDebug($"The following components: '{string.Join(", ", components.Select(x => x.Name))}' will not be started, because the deployment '{ishDeployment.Name}' is not started");
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
