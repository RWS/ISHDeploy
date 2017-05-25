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
﻿using System;
﻿using System.Collections.Generic;
﻿using System.Linq;
﻿using ISHDeploy.Common;
﻿using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Data.Exceptions;
﻿using Microsoft.Web.Administration;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
﻿using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Implements web application management.
    /// </summary>
    /// <seealso cref="IWebAdministrationManager" />
    public class WebAdministrationManager : IWebAdministrationManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The size of salt in bytes (keeping in range of the recommended size of 128-192 bits) 
        /// </summary>
        private const int SaltByteLength = 24;

        /// <summary>
        /// The size of the outputted hash of the password
        /// </summary>
        private const int DerivedKeyLength = 24;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAdministrationManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WebAdministrationManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Recycles specific application pool
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        /// <param name="startIfNotRunning">if set to <c>true</c> then starts application pool if not running.</param>
        public void RecycleApplicationPool(string applicationPoolName, bool startIfNotRunning = false)
        {
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == applicationPoolName);

                if (appPool != null)
                {
                    _logger.WriteDebug("Recycle application pool", applicationPoolName);

                    // Wait while application pool operation is completed
                    if (appPool.State == ObjectState.Stopping || appPool.State == ObjectState.Starting)
                    {
                        WaitOperationCompleted(appPool);
                    }

                    //The app pool is running, so stop it first.
                    if (appPool.State == ObjectState.Started)
                    {
                        _logger.WriteDebug("Application pool is started. Recycle it", applicationPoolName);

                        appPool.Recycle();
                        WaitOperationCompleted(appPool);
                        _logger.WriteVerbose($"Application pool `{applicationPoolName}` has been recycled.");
                    }
                    else if (appPool.State == ObjectState.Stopped && startIfNotRunning)
                    {
                        _logger.WriteDebug("Application pool is stopped. Start it.", applicationPoolName);

                        appPool.Start();
                        WaitOperationCompleted(appPool);
                        _logger.WriteVerbose($"Application pool `{applicationPoolName}` has been started");
                    }
                }
                else
                {
                    // Means system is broken.
                    throw new ArgumentException($"Application pool `{applicationPoolName}` does not exists.");
                }
            }
        }

        /// <summary>
        /// Enables the windows authentication.
        /// </summary>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <exception cref="WindowsAuthenticationModuleIsNotInstalledException"></exception>
        public void EnableWindowsAuthentication(string webSiteName)
        {
            _logger.WriteDebug("Enable WindowsAuthentication for site", webSiteName);
            if (IsWindowsAuthenticationFeatureEnabled())
            {
                using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
                {
                    var config = manager.GetApplicationHostConfiguration();

                    var locationPath = config.GetLocationPaths().FirstOrDefault(x => x.Contains(webSiteName));
                    if (locationPath == null)
                    {
                        throw new WindowsAuthenticationModuleIsNotInstalledException(
                            "WindowsAuthentication module has not been installed");
                    }
                }
                SetWebConfigurationProperty(webSiteName,
                        "system.webServer/security/authentication/windowsAuthentication",
                        WebConfigurationProperty.enabled, true);

                _logger.WriteVerbose("WindowsAuthentication has been enabled");
            }
            else
            {
                throw new WindowsAuthenticationModuleIsNotInstalledException("IIS-WindowsAuthentication feature has not been turned on. You can run command: 'Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication' to enable it");
            }
        }

        /// <summary>
        /// Disables the windows authentication.
        /// </summary>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <exception cref="WindowsAuthenticationModuleIsNotInstalledException"></exception>
        public void DisableWindowsAuthentication(string webSiteName)
        {
            _logger.WriteDebug("Disable WindowsAuthentication for site", webSiteName);
            SetWebConfigurationProperty(webSiteName, "system.webServer/security/authentication/windowsAuthentication",
                WebConfigurationProperty.enabled, false);

            _logger.WriteVerbose("WindowsAuthentication has been disabled");
        }

        /// <summary>
        /// Determines whether IIS-WindowsAuthentication feature enabled or not.
        /// </summary>
        /// <returns>State of WindowsAuthenticationFeature</returns>
        private bool IsWindowsAuthenticationFeatureEnabled()
        {
            _logger.WriteDebug("Checking IIS-WindowsAuthentication feature is turned on or not");

            var psManager = ObjectFactory.GetInstance<IPowerShellManager>();

            var result = psManager.InvokeEmbeddedResourceAsScriptWithResult(
                "ISHDeploy.Data.Resources.Check-WindowsAuthenticationFeatureEnabled.ps1");

            var isFeatureEnabled = bool.Parse(result.ToString());

            _logger.WriteVerbose($"IIS-WindowsAuthentication feature is {(isFeatureEnabled ? "turned on" : "turned off")}");

            return isFeatureEnabled;
        }

        /// <summary>
        /// Stops specific application pool
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        public void StopApplicationPool(string applicationPoolName)
        {
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == applicationPoolName);

                if (appPool != null)
                {
                    _logger.WriteDebug("Stop application pool", applicationPoolName);
                    // Wait while application pool operation is completed
                    if (appPool.State == ObjectState.Stopping || appPool.State == ObjectState.Starting)
                    {
                        WaitOperationCompleted(appPool);
                    }

                    //The app pool is running, so stop it.
                    if (appPool.State == ObjectState.Started)
                    {
                        _logger.WriteDebug("Application pool is started. Stop it", applicationPoolName);

                        appPool.Stop();
                        WaitOperationCompleted(appPool);
                        _logger.WriteVerbose($"Application pool `{applicationPoolName}` has been stopped.");
                    }

                    //The app pool is already stopped.
                    if (appPool.State == ObjectState.Stopped)
                    {
                        _logger.WriteVerbose($"Application pool `{applicationPoolName}` is already stopped.");
                    }
                }
                else
                {
                    // Means system is broken.
                    throw new ArgumentException($"Application pool `{applicationPoolName}` does not exists.");
                }
            }
        }

        /// <summary>
        /// Wait until application pool operation is completed.
        /// </summary>
        /// <param name="appPool">The application pool.</param>
        private void WaitOperationCompleted(ApplicationPool appPool)
        {
            int i = 0;
            _logger.WriteDebug("Wait until application pool change status", appPool.Name);
            while (appPool.State == ObjectState.Stopping || appPool.State == ObjectState.Starting)
            {
                System.Threading.Thread.Sleep(100);
                i++;

                if (i > 100)
                {
                    throw new TimeoutException($"Application pool `{appPool.Name}` for a long time does not change the state. The state is: {appPool.State}");
                }
            }
        }

        /// <summary>
        /// Sets application pool property
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        /// <param name="propertyName">The name of ApplicationPool property.</param>
        /// <param name="value">The value.</param>
        public void SetApplicationPoolProperty(string applicationPoolName, ApplicationPoolProperty propertyName, object value)
        {
            using (var manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                var config = manager.GetApplicationHostConfiguration();
                ConfigurationSection applicationPoolsSection = config.GetSection("system.applicationHost/applicationPools");

                ConfigurationElementCollection applicationPoolsCollection = applicationPoolsSection.GetCollection();

                var poolElement = applicationPoolsCollection.SingleOrDefault(x => x["Name"].ToString() == applicationPoolName);
                if (poolElement != null)
                {
                    var processModelElement = poolElement.ChildElements.Single(x => x.ElementTagName == "processModel");
                    processModelElement.SetAttributeValue(propertyName.ToString(), value);
                }
                else
                {
                    throw new ArgumentException($"Application pool `{applicationPoolName}` does not exists.");
                }

                manager.CommitChanges();
            }
        }

        /// <summary>
        /// Gets application pool property value
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        /// <param name="propertyName">The name of ApplicationPool property.</param>
        /// <returns>
        /// The value by property name.
        /// </returns>
        public object GetApplicationPoolProperty(string applicationPoolName, ApplicationPoolProperty propertyName)
        {
            using (var manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                var config = manager.GetApplicationHostConfiguration();
                ConfigurationSection applicationPoolsSection =
                    config.GetSection("system.applicationHost/applicationPools");

                ConfigurationElementCollection applicationPoolsCollection = applicationPoolsSection.GetCollection();

                var poolElement =
                    applicationPoolsCollection.SingleOrDefault(x => x["Name"].ToString() == applicationPoolName);
                if (poolElement == null)
                {
                    throw new ArgumentException($"Application pool `{applicationPoolName}` does not exists.");
                }

                var processModelElement = poolElement.ChildElements.Single(x => x.ElementTagName == "processModel");
                switch (propertyName)
                {
                    case ApplicationPoolProperty.identityType:
                        return  (ProcessModelIdentityType)processModelElement.GetAttributeValue(propertyName.ToString());
                    default:
                        return processModelElement.GetAttributeValue(propertyName.ToString());
                }
            }
        }

        /// <summary>
        /// Sets web configuration property.
        /// </summary>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <param name="configurationXPath">The xPath to get configuration node.</param>
        /// <param name="propertyName">The name of WebConfiguration property.</param>
        /// <param name="value">The value.</param>
        public void SetWebConfigurationProperty(string webSiteName, string configurationXPath, WebConfigurationProperty propertyName, object value)
        {
            _logger.WriteDebug("Set WebConfiguration property for site", webSiteName, propertyName);

            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                _logger.WriteDebug("Set WebConfiguration property for site", webSiteName, propertyName);

                var config = manager.GetApplicationHostConfiguration();
                var locationPath = config.GetLocationPaths().FirstOrDefault(x => x.Contains(webSiteName));

                var section = config.GetSection(
                    configurationXPath,
                    locationPath);
               
                section[propertyName.ToString()] = value;

                manager.CommitChanges();

                _logger.WriteVerbose($"WebConfiguration property {propertyName} for site `{webSiteName}` has been chenged");
            }
        }

        /// <summary>
        /// Gets web configuration property.
        /// </summary>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <param name="configurationXPath">The xPath to get configuration node.</param>
        /// <param name="propertyName">The name of WebConfiguration property.</param>
        /// <returns>
        /// The value by property name.
        /// </returns>
        public object GetWebConfigurationProperty(string webSiteName, string configurationXPath, WebConfigurationProperty propertyName)
        {
            _logger.WriteDebug("Get WebConfiguration property for site", webSiteName, propertyName);
            object value;
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                _logger.WriteDebug("Set WebConfiguration property for site", webSiteName, propertyName);

                var config = manager.GetApplicationHostConfiguration();
                var locationPath = config.GetLocationPaths().FirstOrDefault(x => x.Contains(webSiteName));

                var section = config.GetSection(
                    configurationXPath,
                    locationPath);


                value = section[propertyName.ToString()];
                _logger.WriteVerbose($"WebConfiguration property {propertyName} for site `{webSiteName}` has been gotten");
            }
            return value;
        }

        /// <summary>
        /// Check application pool is Started or not
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        /// <returns>
        /// True if the state of application pool is Started.
        /// </returns>
        public bool IsApplicationPoolStarted(string applicationPoolName)
        {
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                ApplicationPool appPool = manager.ApplicationPools.FirstOrDefault(ap => ap.Name == applicationPoolName);

                if (appPool != null)
                {
                    _logger.WriteDebug("Get application pool state", applicationPoolName);
                    // Wait while application pool operation is completed
                    if (appPool.State == ObjectState.Stopping || appPool.State == ObjectState.Starting)
                    {
                        WaitOperationCompleted(appPool);
                    }

                    var isStarted = appPool.State == ObjectState.Started;

                    _logger.WriteVerbose($"Application pool `{applicationPoolName}` is {ObjectState.Started}");

                    return isStarted;
                }
                else
                {
                    throw new ArgumentException($"Application pool `{applicationPoolName}` does not exists.");
                }
            }
        }

        /// <summary>
        /// Gets all IIS application pool components.
        /// </summary>
        /// <returns>
        /// The list of IIS application pool components.
        /// </returns>
        public IEnumerable<ISHIISAppPoolComponent> GetAppPoolComponents(params string[] applicationPoolNames)
        {
            if (applicationPoolNames == null || !applicationPoolNames.Any())
            {
                throw new ArgumentException("The parameter `applicationPoolNames` does not contain any values");
            }

            _logger.WriteDebug("Get IIS application pool components");

            var components = new List<ISHIISAppPoolComponent>();
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                components.AddRange(
                    manager.ApplicationPools.Where(ap => applicationPoolNames.Contains(ap.Name))
                    .Select(appPool => 
                        new ISHIISAppPoolComponent
                        {
                            Name = appPool.Name,
                            Status = (ISHIISAppPoolComponentStatus) Enum.Parse(typeof (ISHIISAppPoolComponentStatus), appPool.State.ToString())
                        }));
            }

            return components;
        }
    }
}
