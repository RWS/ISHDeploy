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
        /// <param name="componentsNames">Names of components to be enabled.</param>
        /// <remarks>Don't use this method for background task services</remarks>
        public EnableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, params ISHComponentName[] componentsNames) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Enabling of components");

            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            var components =
               dataAggregateHelper.GetActualStateOfComponents(ishDeployment.Name).Components.Where(x => componentsNames.Contains(x.Name));

            InitializeActions(logger, ishDeployment, components);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">Array with the components to be enabled.</param>
        public EnableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, Models.ISHComponent[] components) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Enabling of components");
            
            InitializeActions(logger, ishDeployment, components);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="backgroundTaskRole">The role of BackgroundTask component to be enabled.</param>
        public EnableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, string backgroundTaskRole) :
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
                if (ishDeployment.Status == ISHDeploymentStatus.Started)
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
            }
            else
            {
                throw new ArgumentException($"The BackgroundTask component with role `{backgroundTaskRole}` does not exist");
            }

            Invoker.AddAction(
                new SaveISHComponentAction(
                    Logger,
                    CurrentISHComponentStatesFilePath,
                    component.Role,
                    true));
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
        /// Initializes the necessary actions for enabling the specified components
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="components">Ordered list with the components to be enabled.</param>
        private void InitializeActions(ILogger logger, Models.ISHDeployment ishDeployment, IEnumerable<Models.ISHComponent> components)
        {
            foreach (var component in components)
            {
                if (ishDeployment.Status == ISHDeploymentStatus.Started && component.Name == ISHComponentName.COMPlus)
                {
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
                        else
                        {
                            Invoker.AddAction(new WriteVerboseAction(Logger, () => (true),
                                $"COM+ component `{comPlusComponent.Name}` was already enabled"));
                        }
                    }
                }

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

            // Start components
            var startOperation = new StartISHComponentOperation(logger, ishDeployment, components);

            Invoker.AddActionsRange(startOperation.Invoker.GetActions());
        }

        /// <summary>
        /// Reorder Components Collection (make sure the lucene service start first and then the crawler
        /// and IIS pools before COM+ components)
        /// </summary>
        /// <param name="components"></param>
        /// <returns>Reoredered collection of components</returns>
        private IEnumerable<Models.ISHComponent> ReorderComponentsCollection(IEnumerable<Models.ISHComponent> components)
        {
            var orderedComponentsCollection = new List<Models.ISHComponent>();
            if (components.Any(x => x.Name == ISHComponentName.Crawler))
            {
                orderedComponentsCollection.Add(components.First(x => x.Name == ISHComponentName.SolrLucene));
                orderedComponentsCollection.AddRange(components.Where(x => x.Name != ISHComponentName.SolrLucene && x.Name != ISHComponentName.COMPlus));
            }
            else
            {
                orderedComponentsCollection.AddRange(components.Where(x => x.Name != ISHComponentName.COMPlus));
            }

            if (components.Any(x => x.Name == ISHComponentName.COMPlus))
            {
                orderedComponentsCollection.Add(components.First(x => x.Name == ISHComponentName.COMPlus));
            }

            return orderedComponentsCollection;
        }
        #endregion
    }
}
