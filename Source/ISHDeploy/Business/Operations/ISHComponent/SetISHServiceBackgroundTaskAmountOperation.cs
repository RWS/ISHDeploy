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
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Sets amount of ISH windows service.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHServiceBackgroundTaskAmountOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The name of default role
        /// </summary>
        private const string DefaultRoleName = "Default";

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceBackgroundTaskAmountOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="amount">The number of TranslationBuilder services in the system.</param>
        /// <param name="role">The role of BackgroundTask service.</param>
        public SetISHServiceBackgroundTaskAmountOperation(ILogger logger, Models.ISHDeployment ishDeployment, int amount, string role) :
            base(logger, ishDeployment)
        {
            if (amount > 10)
            {
                throw new Exception($"The {amount} argument is greater than the maximum allowed range of 10. Supply an argument that is less than or equal to 10 and then try the command again");
            }

            _invoker = new ActionInvoker(logger, $"Setting of amount of {ISHWindowsServiceType.BackgroundTask} windows services");

            // Make sure Vanilla backup of all windows services exists
            _invoker.AddAction(new WindowsServiceVanillaBackUpAction(logger, VanillaPropertiesOfWindowsServicesFilePath, ishDeployment.Name));

            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            var currentComponentsState = dataAggregateHelper.GetComponents(ishDeployment.Name);

            if (string.IsNullOrEmpty(role) || string.Equals(role, DefaultRoleName, StringComparison.CurrentCultureIgnoreCase))
            {
                role = DefaultRoleName;
            }

            List<Models.ISHWindowsService> services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.BackgroundTask).Where(x => Equals(x.Role.ToLower(), role.ToLower())).ToList();
            if (services.Count() > amount)
            {
                // Remove extra services
                var servicesForDeleting = services.Where(serv => serv.Sequence > amount);
                foreach (var service in servicesForDeleting)
                {
                    _invoker.AddAction(new StopWindowsServiceAction(Logger, service));
                    _invoker.AddAction(new RemoveWindowsServiceAction(Logger, service));
                }

                if (amount == 0)
                {
                    _invoker.AddAction(new RemoveISHBackgroundTaskComponentAction(Logger, CurrentISHComponentStatesFilePath, role));
                }
            }
            else if (services.Count() < amount)
            {
                bool templateServiceHasBeenCreated = false;
                // If there is no other service of specified role we take any first one to make it as template for cloning
                var service = services.FirstOrDefault(serv => serv.Sequence == 1);
                if (service == null)
                {
                    service =
                        serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.BackgroundTask).FirstOrDefault();

                    //If there are no services at all, we need to create a service to make it a template, and later delete it
                    if (service == null)
                    {
                        var fileManager = ObjectFactory.GetInstance<IFileManager>();
                        var backedUpWindowsService = fileManager.ReadObjectFromFile<Models.ISHWindowsServiceBackupCollection>(
                                VanillaPropertiesOfWindowsServicesFilePath).Services.FirstOrDefault(x => x.Name.Contains(ISHWindowsServiceType.BackgroundTask.ToString()));

                        _invoker.AddAction(new InstallWindowsServiceAction(Logger, backedUpWindowsService, InputParameters.OSUser, InputParameters.OSPassword));

                        service = new Models.ISHWindowsService { Name = backedUpWindowsService.Name, Sequence = 1, Status = ISHWindowsServiceStatus.Stopped, Role = role, Type = ISHWindowsServiceType.BackgroundTask };

                        // Keep the template service in case if we need to create services with the role "Default"
                        if (!string.Equals(service.Role, DefaultRoleName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            templateServiceHasBeenCreated = true;
                        }
                        else
                        {
                            services = new List<Models.ISHWindowsService> { service };
                        }
                    }
                }

                for (int i = services.Count(); i < amount; i++)
                {
                    _invoker.AddAction(new CloneWindowsServiceAction(Logger, service, i + 1, InputParameters.OSUser, InputParameters.OSPassword, role));
                }

                if (templateServiceHasBeenCreated)
                {
                    _invoker.AddAction(new RemoveWindowsServiceAction(Logger, service));
                }

                if (
                    !currentComponentsState.Components.Any(
                        x =>
                            x.Name == ISHComponentName.BackgroundTask &&
                            string.Equals(x.Role, role, StringComparison.CurrentCultureIgnoreCase)))
                {
                    _invoker.AddAction(new AddISHBackgroundTaskComponentAction(Logger, CurrentISHComponentStatesFilePath,
                        role));
                }
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
