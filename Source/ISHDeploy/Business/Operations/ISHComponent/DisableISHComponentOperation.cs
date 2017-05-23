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
    /// Disable components of deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHComponentOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        protected IActionInvoker Invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="changeStateOfComponentsInTrackingFile">Change state of components in tracking file.</param>
        /// <param name="components">Names of components to be Disabled.</param>
        public DisableISHComponentOperation(ILogger logger, Models.ISHDeployment ishDeployment, bool changeStateOfComponentsInTrackingFile, params ISHComponentName[] components) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Disabling of components");
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            IEnumerable<Models.ISHWindowsService> services;

            var componentsCollection =
                dataAggregateHelper.ReadComponentsFromFile(CurrentISHComponentStatesFilePath.AbsolutePath);
            foreach (var component in componentsCollection.Components.Where(x => components.Contains(x.Name) && x.IsEnabled))
            {
                switch (component.Name)
                {
                    case ISHComponentName.CM :
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
                        // Check if this operation has implications for several Deployments
                        IEnumerable<Models.ISHDeployment> ishDeployments = null;
                        new GetISHDeploymentsAction(logger, string.Empty, result => ishDeployments = result).Execute();


                        var comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
                        var comPlusComponents = comPlusComponentManager.GetCOMPlusComponents();
                        foreach (var comPlusComponent in comPlusComponents)
                        {
                            if (comPlusComponent.Status == ISHCOMPlusComponentStatus.Enabled)
                            {
                                Invoker.AddAction(new WriteWarningAction(Logger, () => (ishDeployments.Count() > 1),
                                    $"The disabling of COM+ component `{comPlusComponent.Name}` has implications across all deployments."));

                                Invoker.AddAction(
                                    new DisableCOMPlusComponentAction(Logger, comPlusComponent.Name));
                            }
                        }
                        break;
                    default:
                        Logger.WriteWarning($"Unsupported component type: {component}");
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
                                (ISHBackgroundTaskRole) Enum.Parse(typeof (ISHBackgroundTaskRole), component.Role),
                                false));
                    }
                    else
                    {
                        Invoker.AddAction(
                            new SaveISHComponentAction(
                                Logger,
                                CurrentISHComponentStatesFilePath,
                                component.Name,
                                false));
                    }
                }
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
