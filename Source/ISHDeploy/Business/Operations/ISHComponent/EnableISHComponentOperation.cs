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
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.COMPlus;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Enable components of deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHComponentOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="changeStateOfComponentsInTrackingFile">Change state of components in tracking file.</param>
        /// <param name="components">Names of components to be enabled.</param>
        public EnableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, bool changeStateOfComponentsInTrackingFile, params ISHComponentName[] components) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Enabling of components");
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            IEnumerable<Models.ISHWindowsService> services;

            var componentsCollection =
               dataAggregateHelper.GetComponents(ishDeployment.Name);

            // Reorder Components Collection (make sure the lucene service start first and then the crawler)
            List<Models.ISHComponent> orderedComponentsCollection = new List<Models.ISHComponent>(); ;
            if (components.Contains(ISHComponentName.Crawler))
            {
                orderedComponentsCollection.Add(componentsCollection.Components.FirstOrDefault(x => x.Name == ISHComponentName.SolrLucene));

                orderedComponentsCollection.AddRange(componentsCollection.Components.Where(x => components.Contains(x.Name) && x.Name != ISHComponentName.SolrLucene));
            }
            else
            {
                orderedComponentsCollection.AddRange(componentsCollection.Components.Where(x => components.Contains(x.Name)));
            }

            foreach (var component in orderedComponentsCollection)
            {
                switch (component.Name)
                {
                    case ISHComponentName.CM :
                        Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.CMAppPoolName, true));
                        break;
                    case ISHComponentName.WS:
                        Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.WSAppPoolName, true));
                        break;
                    case ISHComponentName.STS:
                        Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName, true));
                        break;
                    case ISHComponentName.TranslationBuilder:
                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationBuilder);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
                        }
                        break;
                    case ISHComponentName.TranslationOrganizer:
                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationOrganizer);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(
                                new StartWindowsServiceAction(Logger, service));
                        }
                        break;
                    case ISHComponentName.COMPlus:
                        // Check if this operation has implications for several Deployments
                        IEnumerable<Models.ISHDeployment> ishDeployments = null;
                        new GetISHDeploymentsAction(logger, string.Empty, result => ishDeployments = result).Execute();

                        var comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
                        var comPlusComponents = comPlusComponentManager.GetCOMPlusComponents();
                        foreach (var comPlusComponent in comPlusComponents)
                        {
                            if (comPlusComponent.Status == ISHCOMPlusComponentStatus.Disabled)
                            {
                                Invoker.AddAction(new WriteWarningAction(Logger, () => (ishDeployments.Count() > 1),
                                    $"The enabling of COM+ component `{comPlusComponent.Name}` has implications across all deployments."));

                                Invoker.AddAction(
                                    new EnableCOMPlusComponentAction(Logger, comPlusComponent.Name));
                            }
                        }
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
                        services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.BackgroundTask);
                        foreach (var service in services)
                        {
                            Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
                        }
                        break;
                    default:
                        Logger.WriteDebug($"Unsupported component type: {component.Name}");
                        break;
                }

                if (changeStateOfComponentsInTrackingFile)
                {
                    if (component.Name == ISHComponentName.BackgroundTask)
                    {
                        Invoker.AddAction(
                            new SaveISHComponentAction(
                                Logger,
                                CurrentISHComponentStatesFilePath,
                                component.Role,
                                true));
                    }
                    else
                    {
                        Invoker.AddAction(
                            new SaveISHComponentAction(
                                Logger,
                                CurrentISHComponentStatesFilePath,
                                component.Name,
                                true));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="changeStateOfComponentsInTrackingFile">Change state of components in tracking file.</param>
        /// <param name="backgroundTaskRole">The role of BackgroundTask component to be Disabled.</param>
        public EnableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, bool changeStateOfComponentsInTrackingFile, string backgroundTaskRole) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Enabling of BackgroundTask component with role `{backgroundTaskRole}`");
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            var componentsCollection =
               dataAggregateHelper.ReadComponentsFromFile(CurrentISHComponentStatesFilePath.AbsolutePath);

            var component = componentsCollection[ISHComponentName.BackgroundTask, backgroundTaskRole];

            if (component != null)
            {
                var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.BackgroundTask);
                foreach (var service in services.Where(x => string.Equals(x.Role, component.Role, StringComparison.CurrentCultureIgnoreCase)))
                {
                    Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
                }
            }
            else
            {
                throw new ArgumentException($"The BackgroundTask component with role `{backgroundTaskRole}` does not exist");
            }

            if (changeStateOfComponentsInTrackingFile)
            {
                Invoker.AddAction(
                    new SaveISHComponentAction(
                        Logger,
                        CurrentISHComponentStatesFilePath,
                        component.Role,
                        true));
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
