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
using System.Configuration.Install;
using System.Linq;
using System.Management;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using System.ServiceProcess;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.Backup;

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
        /// The Trisoft registry manager.
        /// </summary>
        private readonly ITrisoftRegistryManager _trisoftRegistryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WindowsServiceManager(ILogger logger)
        {
            _logger = logger;
            _psManager = ObjectFactory.GetInstance<IPowerShellManager>();
            _trisoftRegistryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
        }

        /// <summary>
        /// Starts specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        public void StartWindowsService(string serviceName)
        {
            _logger.WriteDebug("Start Windows service", serviceName);
            var service = new ServiceController(serviceName);
            if (service.Status != ServiceControllerStatus.Running)
            {
                service.Start();
                WaitForStatus(service, ServiceControllerStatus.Running);
                _logger.WriteVerbose($"Windows service `{serviceName}` has been started");
            }
            else
            {
                _logger.WriteVerbose($"Windows service `{serviceName}` was already started");
            }
        }

        /// <summary>
        /// Stops specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        public void StopWindowsService(string serviceName)
        {
            _logger.WriteDebug("Stop Windows service", serviceName);
            var service = new ServiceController(serviceName);
            if (service.Status != ServiceControllerStatus.Stopped)
            {
                service.Stop();
                WaitForStatus(service, ServiceControllerStatus.Stopped);
                _logger.WriteVerbose($"Windows service `{serviceName}` has been stopped");
            }
            else
            {
                _logger.WriteVerbose($"Windows service `{serviceName}` was already stopped");
            }
        }

        /// <summary>
        /// Waiting of windows service status
        /// </summary>
        /// <param name="service">The windows service controller</param>
        /// <param name="status">The service status</param>
        private void WaitForStatus(ServiceController service, ServiceControllerStatus status)
        {
            int numberOfTries = 0;
            while (service.Status != status && numberOfTries < 6)
            {
                try
                {
                    service.WaitForStatus(status, TimeSpan.FromMinutes(5));
                }
                catch (Exception)
                {
                    _logger.WriteVerbose($"The service `{service.DisplayName}` does not change the status for too long");
                    numberOfTries++;

                    if (service.Status != status && numberOfTries >= 6)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all windows services of deployment of specified type.
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
                        .Select(service =>
                            type == ISHWindowsServiceType.BackgroundTask ?
                                new ISHBackgroundTaskWindowsService
                                {
                                    Name = service.ServiceName,
                                    Type = type,
                                    Status = (ISHWindowsServiceStatus)Enum.Parse(typeof(ISHWindowsServiceStatus), service.Status.ToString()),
                                    Sequence =
                                        service.ServiceName.EndsWith(serviceNameAlias) ?
                                        1 :
                                        (int)Enum.Parse(typeof(ISHWindowsServiceSequence), service.ServiceName.Split(' ').Last()),
                                    Role = type == ISHWindowsServiceType.BackgroundTask ? GetWindowsServiceProperties(service.ServiceName).Properties.Single(x => x.Name == "PathName").Value.ToString().Split(' ').Last() : string.Empty
                                }
                                :
                                new ISHWindowsService
                                {
                                    Name = service.ServiceName,
                                    Type = type,
                                    Status = (ISHWindowsServiceStatus)Enum.Parse(typeof(ISHWindowsServiceStatus), service.Status.ToString()),
                                    Sequence =
                                        service.ServiceName.EndsWith(serviceNameAlias) ?
                                        1 :
                                        (int)Enum.Parse(typeof(ISHWindowsServiceSequence), service.ServiceName.Split(' ').Last())
                                }
                            )
                        .ToList());
            }

            return services;
        }

        /// <summary>
        /// Gets all windows services of deployment of all types.
        /// </summary>
        /// <param name="deploymentName">ISH deployment name.</param>
        /// <returns>
        /// The all windows services of deployment.
        /// </returns>
        public IEnumerable<ISHWindowsService> GetAllServices(string deploymentName)
        {
            return GetServices(deploymentName, (ISHWindowsServiceType[]) Enum.GetValues(typeof (ISHWindowsServiceType)));
        }

        /// <summary>
        /// Gets all BackgroundTask windows services.
        /// </summary>
        /// <param name="deploymentName">ISH deployment name.</param>
        /// <returns>
        /// The BackgroundTask windows services of deployment of specified type.
        /// </returns>
        public IEnumerable<ISHBackgroundTaskWindowsService> GetISHBackgroundTaskWindowsServices(string deploymentName)
        {
            var services =
                GetServices(deploymentName, ISHWindowsServiceType.BackgroundTask)
                    .Select(x => x as ISHBackgroundTaskWindowsService);

            return services;
        }

        /// <summary>
        /// Check windows service is Started or not
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        /// <returns>
        /// True if the state of windows service is Manual or Auto.
        /// </returns>
        public bool IsWindowsServiceStarted(string serviceName)
        {
            _logger.WriteDebug("Get windows service state", serviceName);

            var service = ServiceController.GetServices()
                       .FirstOrDefault(x => string.Equals(x.ServiceName, serviceName, StringComparison.CurrentCultureIgnoreCase));

            if (service == null)
            {
                throw new Exception($"Windows service that matches to `{serviceName}` does not exists.");
            }

            var isEnabled = service.Status == ServiceControllerStatus.Running;

            _logger.WriteVerbose($"Windows service `{serviceName}` is {service.Status}");

            return isEnabled;
        }

        /// <summary>
        /// Removes specific windows service
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        public void RemoveWindowsService(string serviceName)
        {
            StopWindowsService(serviceName);
            var serviceInstallerObj = new ServiceInstaller
            {
                Context = new InstallContext(),
                ServiceName = serviceName
            };
            serviceInstallerObj.Uninstall(null);
        }

        /// <summary>
        /// Clones specific windows service
        /// </summary>
        /// <param name="service">The windows service to be cloned.</param>
        /// <param name="sequence">The sequence of new service.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="role">The role of BackgroundTask service.</param>
        /// <returns>
        /// The name of new service that have been created.
        /// </returns>
        public string CloneWindowsService(ISHWindowsService service, int sequence, string userName, string password, string role)
        {
            _logger.WriteDebug("Clone windows service", service.Name);
            var newServiceName = service.Name.Replace(((ISHWindowsServiceSequence)service.Sequence).ToString(), ((ISHWindowsServiceSequence)sequence).ToString());

            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery($"SELECT * FROM Win32_Service WHERE Name = '{service.Name}'");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wqlObjectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

            var pathToExecutable = string.Empty;
            foreach (var managementObject in managementObjectCollection)
            {
                pathToExecutable = managementObject.GetPropertyValue("PathName").ToString();

                if (service is ISHBackgroundTaskWindowsService)
                {
                    if (string.IsNullOrEmpty(role))
                    {
                        throw new ArgumentException("Role cann't be null or empty string");
                    }

                    if (!newServiceName.ToLower().Contains(role.ToLower()) && !string.Equals(role, "default", StringComparison.CurrentCultureIgnoreCase))
                    {
                        newServiceName = newServiceName.Replace(service.Type.ToString(),
                            $"{service.Type} {role}");
                    }

                    if (string.Equals(role, "default", StringComparison.CurrentCultureIgnoreCase))
                    {
                        newServiceName = newServiceName.Replace($"{service.Type} {((ISHBackgroundTaskWindowsService)service).Role}", service.Type.ToString());
                    }

                    pathToExecutable = pathToExecutable.Replace(service.Name, newServiceName);

                    var roleFromPathToExecutable = pathToExecutable.Split(' ').Last();
                    if (!string.Equals(role, roleFromPathToExecutable, StringComparison.CurrentCultureIgnoreCase))
                    {
                        pathToExecutable = pathToExecutable.Replace(roleFromPathToExecutable, role);
                    }
                }

                pathToExecutable = pathToExecutable.Replace(service.Name, newServiceName);
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
        /// Creates windows service
        /// </summary>
        /// <param name="service">The windows service to be created.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The name of new service that have been created.
        /// </returns>
        public void InstallWindowsService(ISHWindowsServiceBackup service, string userName, string password)
        {
            _logger.WriteDebug("Create windows service", service.Name);

            _psManager.InvokeEmbeddedResourceAsScriptWithResult("ISHDeploy.Data.Resources.Install-WindowsService.ps1",
                new Dictionary<string, string>
                {
                    { "$name", service.Name },
                    { "$displayName", service.WindowsServiceManagerProperties.Properties.Single(x => x.Name == "DisplayName").Value.ToString() },
                    { "$description", service.WindowsServiceManagerProperties.Properties.Single(x => x.Name == "Description").Value.ToString() },
                    { "$pathToExecutable", service.WindowsServiceManagerProperties.Properties.Single(x => x.Name == "PathName").Value.ToString() },
                    { "$username", userName },
                    { "$password", password }
                });

            _logger.WriteVerbose($"New service `{service.Name}` has been created");
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

            string fullServiceName = $"Win32_Service.Name='{serviceName}'";
            ManagementObject managementObject = new ManagementObject(fullServiceName);

            object result = managementObject.InvokeMethod("Change", new object[]
              { null, null, null, null, null, null, userName, password, null, null, null });
            if ((uint)result != 0)
            {
                throw new Exception($"Setting credentials for the service '{serviceName}' failed with {result}");
            }
            _logger.WriteVerbose($"Credentials for the service `{serviceName}` has been changed");
        }

        /// <summary>
        /// Set the startup type of the windows service (Manual, Automatic, Automatic (Delayed start),...)
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        /// <param name="startupType">The new startup type of the service.</param>
        public void SetWindowsServiceStartupType(string serviceName, ISHWindowsServiceStartupType startupType)
        {
            _logger.WriteDebug("Set windows service startup type", serviceName);

            // Set the start mode (Manual or Automatic)
            string fullServiceName = $"Win32_Service.Name='{serviceName}'";
            ManagementObject managementObject = new ManagementObject(fullServiceName);

            if (startupType == ISHWindowsServiceStartupType.Automatic)
            {
                string registryKey = $@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{serviceName}";
                if (_trisoftRegistryManager.GetValue(registryKey, "DelayedAutostart", 0).ToString() == "0")
                {
                    // Note: The windows service is always set to delayed start.
                    _trisoftRegistryManager.SetValue(registryKey, "DelayedAutostart", 1);
                }
            }

            object result = managementObject.InvokeMethod("Change", new object[]
                { null, null, null, null, startupType, null, null, null, null, null, null });

            if ((uint)result != 0)
            {
                throw new Exception($"Setting start mode '{startupType}' for the service '{serviceName}' failed with {result}");
            }
            _logger.WriteVerbose($"Startup type for the service '{serviceName}' has been changed");
        }

        /// <summary>
        /// Remove (all) dependencies for the windows service
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        public void RemoveWindowsServiceDependency(string serviceName)
        {
            _logger.WriteDebug("Remove dependencies of the service", serviceName);

            string fullServiceName = $"Win32_Service.Name='{serviceName}'";
            ManagementObject managementObject = new ManagementObject(fullServiceName);

            string[] dependencies = { "" };

            object result = managementObject.InvokeMethod("Change", new object[]
              { null, null, null, null, null, null, null, null, null, null, dependencies });
            if ((uint)result != 0)
            {
                throw new Exception($"Removing dependencies for the service '{serviceName}' failed with {result}");
            }
            _logger.WriteVerbose($"Dependencies of the service '{serviceName}' have been removed");
        }

        /// <summary>
        /// Gets all dependencies for the windows services of deployment of specified type.
        /// </summary>
        /// <param name="deploymentName">ISH deployment name.</param>
        /// <param name="serviceType">Type of deployment service.</param>
        /// <returns>
        /// The dependencies for the specified type.
        /// </returns>
        public IEnumerable<string> GetDependencies(string deploymentName, ISHWindowsServiceType serviceType)
        {
            var dependencies = new List<string>();

            var services = GetServices(deploymentName, serviceType);
            foreach (var service in services)
            {
                object result = Registry.LocalMachine.OpenSubKey("SYSTEM").OpenSubKey("CurrentControlSet").OpenSubKey("Services").OpenSubKey(service.Name).GetValue(RegistryValueName.DependOnService.ToString());
                if (result != null)
                {
                    string[] dependsOnServices = (string[])result;
                    foreach (var dependency in dependsOnServices)
                    {
                        if (dependency != String.Empty && !dependencies.Contains(dependency))
                        {
                            dependencies.Add(dependency);
                        }
                    }
                }
            }

            return dependencies;
        }

        /// <summary>
        /// Gets properties of windows service
        /// </summary>
        /// <param name="serviceName">The name of windows service.</param>
        /// <returns>
        /// Properties of windows service.
        /// </returns>
        public PropertyCollection GetWindowsServiceProperties(string serviceName)
        {
            _logger.WriteDebug("Get properties of windows service", serviceName);

            WqlObjectQuery wqlObjectQuery =
                new WqlObjectQuery($"SELECT * FROM Win32_Service WHERE Name = '{serviceName}'");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wqlObjectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

            var result = new PropertyCollection();
            foreach (var managementObject in managementObjectCollection)
            {
                foreach (var prop in managementObject.Properties)
                {
                    if (prop.Value != null)
                    {
                        result.Properties.Add(new Property { Name = prop.Name, Value = prop.Value });
                    }
                }
            }

            _logger.WriteVerbose($"Properties for service `{serviceName}` has been got");
            return result;
        }
    }
}
