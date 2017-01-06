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
﻿using System.IO;
﻿using System.Linq;
﻿using System.Management.Automation;
﻿using System.Reflection;
﻿using ISHDeploy.Data.Exceptions;
﻿using Microsoft.Web.Administration;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

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

                    var windowsAuthenticationSection = config.GetSection(
                        "system.webServer/security/authentication/windowsAuthentication",
                        locationPath);
                    windowsAuthenticationSection["enabled"] = true;

                    manager.CommitChanges();

                    _logger.WriteVerbose("WindowsAuthentication has been enabled");
                }
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
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                _logger.WriteDebug("Disable WindowsAuthentication for site", webSiteName);

                var config = manager.GetApplicationHostConfiguration();
                var locationPath = config.GetLocationPaths().FirstOrDefault(x => x.Contains(webSiteName));

                var windowsAuthenticationSection = config.GetSection(
                    "system.webServer/security/authentication/windowsAuthentication",
                    locationPath);
                windowsAuthenticationSection["enabled"] = false;

                manager.CommitChanges();

                _logger.WriteVerbose("WindowsAuthentication has been disabled");
            }
        }

        /// <summary>
        /// Determines whether IIS-WindowsAuthentication feature enabled or not.
        /// </summary>
        /// <returns></returns>
        private bool IsWindowsAuthenticationFeatureEnabled()
        {
            bool isFeatureEnabled = false;

            _logger.WriteDebug("Checking IIS-WindowsAuthentication feature is turned on or not");

            using (var ps = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                // Read Check-WindowsAuthenticationFeatureEnabled.ps1 script
                using (var resourceReader = Assembly.GetExecutingAssembly().GetManifestResourceStream("ISHDeploy.Data.Resources.Check-WindowsAuthenticationFeatureEnabled.ps1"))
                {
                    using (var reader = new StreamReader(resourceReader))
                    {
                        ps.AddScript(reader.ReadToEnd());
                    }
                }

                foreach (PSObject result in ps.Invoke())
                {
                    foreach (var verbose in ps.Streams.Verbose.ReadAll())
                    {
                        _logger.WriteVerbose(verbose.Message);
                    }

                    foreach (var warning in ps.Streams.Warning.ReadAll())
                    {
                        _logger.WriteWarning(warning.Message);
                    }

                    isFeatureEnabled = result != null && bool.Parse(result.ToString());
                }
            }

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
        /// Sets identity type of specific application pool as ApplicationPoolIdentity
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        public void SetApplicationPoolIdentityType(string applicationPoolName)
        {
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                _logger.WriteDebug("Set ApplicationPoolIdentity identity type", applicationPoolName);

                var config = manager.GetApplicationHostConfiguration();


                ConfigurationSection applicationPoolsSection = config.GetSection("system.applicationHost/applicationPools");

                ConfigurationElementCollection applicationPoolsCollection = applicationPoolsSection.GetCollection();

                var stsPoolElement = applicationPoolsCollection.SingleOrDefault(x => x["Name"].ToString() == applicationPoolName);
                if (stsPoolElement != null)
                {
                    var processModelElement =
                        stsPoolElement.ChildElements.Single(x => x.ElementTagName == "processModel");
                    processModelElement.SetAttributeValue("identityType", "ApplicationPoolIdentity");
                }
                else
                {
                    throw new ArgumentException($"Application pool `{applicationPoolName}` does not exists.");
                }

                manager.CommitChanges();
                _logger.WriteVerbose($"The identity type for application poll `{applicationPoolName} has been changed");
            }
        }

        /// <summary>
        /// Sets identity type of specific application pool as Custom account
        /// </summary>
        /// <param name="applicationPoolName">Name of the application pool.</param>
        public void SetSpecificUserIdentityType(string applicationPoolName)
        {
            using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
            {
                _logger.WriteDebug("Set application pool SpecificUser identity type", applicationPoolName);

                var config = manager.GetApplicationHostConfiguration();


                ConfigurationSection applicationPoolsSection = config.GetSection("system.applicationHost/applicationPools");

                ConfigurationElementCollection applicationPoolsCollection = applicationPoolsSection.GetCollection();

                var stsPoolElement = applicationPoolsCollection.SingleOrDefault(x => x["Name"].ToString() == applicationPoolName);
                if (stsPoolElement != null)
                {
                    var processModelElement =
                        stsPoolElement.ChildElements.Single(x => x.ElementTagName == "processModel");
                    processModelElement.SetAttributeValue("identityType", "SpecificUser");
                }
                else
                {
                    throw new ArgumentException($"Application pool `{applicationPoolName}` does not exists.");
                }

                manager.CommitChanges();
                _logger.WriteVerbose($"The identity type for application poll `{applicationPoolName}` has been changed");
            }
        }
    }
}
