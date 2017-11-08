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
using ISHDeploy.Common.Models.Backup;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.COMPlus;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Actions.Registry;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Stop components of deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class StopISHComponentOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="componentsNames">Names of components to be Stopped.</param>
        /// <remarks>Don't use this method for background task services</remarks>
        public StopISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, params ISHComponentName[] componentsNames) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Disabling of components");
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            var components =
                dataAggregateHelper.GetActualStateOfComponents(ishDeployment.Name).Components.Where(x => componentsNames.Contains(x.Name)); ;

            InitializeActions(logger, ishDeployment, components);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">Array of components to be stopped.</param>
        /// <remarks>Don't use this method for background task services</remarks>
        public StopISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, IEnumerable<Models.ISHComponent> components) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Disabling of components");

            InitializeActions(logger, ishDeployment, components);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StopISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="backgroundTaskRole">The role of BackgroundTask component to be Stopped.</param>
        public StopISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, string backgroundTaskRole) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Disabling of BackgroundTask component with role `{backgroundTaskRole}`");
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            var componentsCollection =
                dataAggregateHelper.GetExpectedStateOfComponents(CurrentISHComponentStatesFilePath.AbsolutePath);

            var component = componentsCollection[ISHComponentName.BackgroundTask, backgroundTaskRole];

            if (component != null)
            {
                var services = serviceManager.GetISHBackgroundTaskWindowsServices(ishDeployment.Name);
                foreach (var service in services.Where(x => string.Equals(x.Role, component.Role, StringComparison.CurrentCultureIgnoreCase)))
                {
                    Invoker.AddAction(new StopWindowsServiceAction(Logger, service));
                }
            }
            else
            {
                throw new ArgumentException($"The BackgroundTask component with role `{backgroundTaskRole}` does not exist");
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }

        #region Private methods
        /// <summary>
        /// Initializes the necessary actions for disabling the specified components
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">Ordered list with the components to be Stopped.</param>
        private void InitializeActions(ILogger logger, Models.ISHDeployment ishDeployment, IEnumerable<Models.ISHComponent> components)
        {
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            var orderedComponentsCollection = ReorderComponentsCollection(components);

            foreach (var component in orderedComponentsCollection)
            {
                IEnumerable<Models.ISHWindowsService> services;

                switch (component.Name)
                {
                    case ISHComponentName.CM:
                        Invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.CMAppPoolName));
                        break;
                    case ISHComponentName.WS:
                        Invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.WSAppPoolName));
                        break;
                    case ISHComponentName.STS:
                        Invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.STSAppPoolName));
                        break;
                    case ISHComponentName.TranslationBuilder:
                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationBuilder);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new StopWindowsServiceAction(Logger, service));
                        }
                        break;
                    case ISHComponentName.TranslationOrganizer:
                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationOrganizer);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(
                                new StopWindowsServiceAction(Logger, service));
                        }
                        break;
                    case ISHComponentName.COMPlus:
                        var comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();

                        if (comPlusComponentManager.IsComPlusComponentRunning(TrisoftInfoShareAuthorComPlusApplicationName))
                        {
                            // Check if this operation has implications for several Deployments
                            IEnumerable<Models.ISHDeployment> ishDeployments = null;
                            new GetISHDeploymentsAction(logger, string.Empty, result => ishDeployments = result).Execute();
                            Invoker.AddAction(new WriteWarningAction(Logger, () => (ishDeployments.Count() > 1),
                                $"The stopping of COM+ component `{TrisoftInfoShareAuthorComPlusApplicationName}` has implications across all deployments."));

                            Invoker.AddAction(
                                new ShutdownCOMPlusComponentAction(Logger, TrisoftInfoShareAuthorComPlusApplicationName));
                        }
                        break;
                    case ISHComponentName.Crawler:
                        Invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));
                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.Crawler);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new StopWindowsServiceAction(Logger, service));
                            // Remove dependencies between Crawler and SolrLucene
                            Invoker.AddAction(new SetRegistryValueAction(logger, new RegistryValue { Key = string.Format(RegWindowsServicesRegistryPathPattern, service.Name), ValueName = RegistryValueName.DependOnService, Value = string.Empty }));
                        }
                        break;
                    case ISHComponentName.SolrLucene:
                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.SolrLucene);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new StopWindowsServiceAction(Logger, service));
                        }
                        break;
                    case ISHComponentName.BackgroundTask:
                        var backgroundTaskServices = serviceManager.GetISHBackgroundTaskWindowsServices(ishDeployment.Name);
                        foreach (var backgroundTaskService in backgroundTaskServices)
                        {
                            if (backgroundTaskService.Role.Equals(component.Role, StringComparison.InvariantCultureIgnoreCase))
                            {
                                Invoker.AddAction(new StopWindowsServiceAction(Logger, backgroundTaskService));
                            }
                        }
                        break;
                    default:
                        Logger.WriteDebug($"Unsupported component type: {component.Name}");
                        break;
                }
            }
        }

        /// <summary>
        /// Reorder Components Collection (make sure the crawler service stops first and then the lucene
        /// and COM+ components before IIS pools)
        /// </summary>
        /// <param name="components"></param>
        /// <returns>Reoredered collection of components</returns>
        private IEnumerable<Models.ISHComponent> ReorderComponentsCollection(IEnumerable<Models.ISHComponent> components)
        {
            var orderedComponentsCollection = new List<Models.ISHComponent>();
            if (components.Any(x => x.Name == ISHComponentName.COMPlus))
            {
                orderedComponentsCollection.Add(components.First(x => x.Name == ISHComponentName.COMPlus));
            }

            if (components.Any(x => x.Name == ISHComponentName.SolrLucene))
            {
                orderedComponentsCollection.Add(components.First(x => x.Name == ISHComponentName.Crawler));
                orderedComponentsCollection.AddRange(components.Where(x => x.Name != ISHComponentName.Crawler && x.Name != ISHComponentName.COMPlus));
            }
            else
            {
                orderedComponentsCollection.AddRange(components.Where(x => x.Name != ISHComponentName.COMPlus));
            }

            return orderedComponentsCollection;
        }
        #endregion

    }
}
