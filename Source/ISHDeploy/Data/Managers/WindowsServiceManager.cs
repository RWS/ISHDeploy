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
using System.Management;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using System.ServiceProcess;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Exceptions;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    /// <seealso cref="IWindowsServiceManager" />
    public class WindowsServiceManager : IWindowsServiceManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The powershell manager.
        /// </summary>
        private readonly IPowerShellManager _psManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WindowsServiceManager(ILogger logger)
        {
            _logger = logger;
            _psManager = ObjectFactory.GetInstance<IPowerShellManager>();
        }

        /// <summary>
        /// Starts specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        public void StartWindowsService(string serviceName)
        {
            try
            {
                _logger.WriteDebug("Start Windows service", serviceName);
                var service = new ServiceController(serviceName);
                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    _logger.WriteVerbose($"Windows service `{serviceName}` has been started");
                }
                else
                {
                    _logger.WriteVerbose($"Windows service `{serviceName}` was already started");
                }
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                throw new ISHWindowsServiceTimeoutException(serviceName, ex);
            }
        }

        /// <summary>
        /// Stops specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        public void StopWindowsService(string serviceName)
        {
            try
            {
                _logger.WriteDebug("Stop Windows service", serviceName);
                var service = new ServiceController(serviceName);
                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    _logger.WriteVerbose($"Windows service `{serviceName}` has been stopped");
                }
                else
                {
                    _logger.WriteVerbose($"Windows service `{serviceName}` was already stopped");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.InnerException);
            }
        }

        /// <summary>
        /// Gets all windows services of specified type.
        /// </summary>
        /// <param name="deploymentName">ISH deployment name.</param>
        /// <param name="types">Types of deployment service.</param>
        /// <returns>
        /// The windows services of deployment of specified type.
        /// </returns>
        public IEnumerable<ISHWindowsService> GetServices(string deploymentName, params ISHWindowsServiceType[] types)
        {
            var services = new List<ISHWindowsService>();

            foreach (var type in types)
            {
                string serviceNameAlias = $"{deploymentName} {type}";
                services.AddRange(
                    ServiceController.GetServices()
                        .Where(x => x.ServiceName.Contains(serviceNameAlias))
                        .Select(service => new ISHWindowsService {
                            Name = service.ServiceName,
                            Type = type,
                            Status = (ISHWindowsServiceStatus)Enum.Parse(typeof(ISHWindowsServiceStatus), service.Status.ToString()),
                            Sequence =
                                service.ServiceName.EndsWith(serviceNameAlias) ?
                                1 :
                                (int)Enum.Parse(typeof(ISHWindowsServiceSequence), service.ServiceName.Split(' ').Last())
                        })
                        .ToList());
            }

            return services;
        }

        /// <summary>
        /// Check windows service is Started or not
        /// </summary>
        /// <param name="deploymentName">ISH deployment name.</param>
        /// <param name="type">Type of deployment service.</param>
        /// <returns>
        /// True if the state of windows service is Manual or Auto.
        /// </returns>
        public bool IsWindowsServiceStarted(string deploymentName, ISHWindowsServiceType type)
        {
            string serviceNameAlias = $"{deploymentName} {type}";
            _logger.WriteDebug("Get windows service state", serviceNameAlias);


            var service = ServiceController.GetServices()
                       .FirstOrDefault(x => x.ServiceName.Contains(serviceNameAlias));

            if (service == null)
            {
                throw new Exception($"Windows service that matches to `{serviceNameAlias}` does not exists.");
            }

            var isEnabled = service.Status == ServiceControllerStatus.Running;

            _logger.WriteVerbose($"Windows service `{serviceNameAlias}` is {service.Status}");

            return isEnabled;
        }

        /// <summary>
        /// Removes specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        public void RemoveWindowsService(string serviceName)
        {
            StopWindowsService(serviceName);
            _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Uninstall-WindowsService.ps1",
                new Dictionary<string, string>
                {
                    { "$name", serviceName }
                });
        }

        /// <summary>
        /// Clones specific windows service
        /// </summary>
        /// <param name="service">The windows service to be cloned.</param>
        /// <param name="sequence">The sequence of new service.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The name of new service that have been created.
        /// </returns>
        public string CloneWindowsService(ISHWindowsService service, int sequence, string userName, string password)
        {
            _logger.WriteDebug("Clone windows service", service.Name);
            var newServiceName = service.Name.Replace(((ISHWindowsServiceSequence)service.Sequence).ToString(), ((ISHWindowsServiceSequence)sequence).ToString());

            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery($"SELECT * FROM Win32_Service WHERE Name = '{service.Name}'");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wqlObjectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

            var pathToExecutable = string.Empty;
            foreach (var managementObject in managementObjectCollection)
            {
                pathToExecutable = managementObject.GetPropertyValue("PathName").ToString().Replace("\"", "`\"").Replace(service.Name, newServiceName);
            }

            _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Install-WindowsService.ps1", 
                new Dictionary<string, string>
                {
                    { "$name", newServiceName },
                    { "$displayName", newServiceName },
                    { "$description", $"{newServiceName} ({sequence})" },
                    { "$pathToExecutable", pathToExecutable },
                    { "$username", userName },
                    { "$password", password }
                });

            _logger.WriteVerbose($"New service `{newServiceName}` has been created");
            return newServiceName;
        }

        /// <summary>
        /// Set windows service credentials
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>

        public void SetWindowsServiceCredentials(string serviceName, string userName, string password)
        {
            _logger.WriteDebug("Set windows service credentials", serviceName);

            _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Set-WindowsServiceCredentials.ps1",
                new Dictionary<string, string>
                {
                    { "$name", serviceName },
                    { "$username", userName },
                    { "$password", password }
                },
                "Setting windows service credentials");

            _logger.WriteVerbose($"Credentials for the service `{serviceName}` has been chenged");
        }
    }
}
