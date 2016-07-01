/**
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
using System.Linq;
﻿using ISHDeploy.Data.Exceptions;
﻿using Microsoft.Web.Administration;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
﻿using Microsoft.Dism;

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
                    _logger.WriteDebug($"Recycle application pool: `{applicationPoolName}`");

                    // Wait while application pool operation is completed
                    if (appPool.State == ObjectState.Stopping || appPool.State == ObjectState.Starting)
                    {
                        WaitOperationCompleted(appPool);
                    }

                    //The app pool is running, so stop it first.
                    if (appPool.State == ObjectState.Started)
                    {
                        _logger.WriteDebug($"Application pool `{applicationPoolName}` recycle.");

                        appPool.Recycle();
                        WaitOperationCompleted(appPool);
                        _logger.WriteVerbose($"Application pool `{applicationPoolName}` recycled.");
                    }
                    else if (appPool.State == ObjectState.Stopped && startIfNotRunning)
                    {
                        _logger.WriteDebug($"Application pool `{applicationPoolName}` is stopped. Start it.");

                        appPool.Start();
                        WaitOperationCompleted(appPool);
                        _logger.WriteVerbose($"Application pool `{applicationPoolName}` started");
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
            if (IsWindowsAuthenticationFeatureEnabled())
            {
                using (ServerManager manager = ServerManager.OpenRemote(Environment.MachineName))
                {
                    _logger.WriteDebug($"Enable WindowsAuthentication for site: `{webSiteName}`");

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

                    var anonymousAuthenticationSection = config.GetSection(
                        "system.webServer/security/authentication/anonymousAuthentication",
                        locationPath);
                    anonymousAuthenticationSection["enabled"] = false;
                    manager.CommitChanges();
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
                _logger.WriteDebug($"Enable WindowsAuthentication for site: `{webSiteName}`");

                var config = manager.GetApplicationHostConfiguration();
                var locationPath = config.GetLocationPaths().FirstOrDefault(x => x.Contains(webSiteName));

                var windowsAuthenticationSection = config.GetSection(
                    "system.webServer/security/authentication/windowsAuthentication",
                    locationPath);

                windowsAuthenticationSection["enabled"] = false;

                var anonymousAuthenticationSection = config.GetSection(
                    "system.webServer/security/authentication/anonymousAuthentication",
                    locationPath);

                anonymousAuthenticationSection["enabled"] = true;
                manager.CommitChanges();

            }
        }

        /// <summary>
        /// Determines whether IIS-WindowsAuthentication feature enabled or not.
        /// </summary>
        /// <returns></returns>
        private bool IsWindowsAuthenticationFeatureEnabled()
        {
            bool isEnabled = false;
            DismApi.Initialize(DismLogLevel.LogErrors);
            using (DismSession session = DismApi.OpenOnlineSession())
            {
                isEnabled =
                    DismApi.GetFeatures(session)
                        .Any(
                            x =>
                                x.FeatureName == "IIS-WindowsAuthentication" &&
                                x.State == DismPackageFeatureState.Installed);
            }
            DismApi.Shutdown();
            return isEnabled;
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
                    _logger.WriteDebug($"Stop application pool: `{applicationPoolName}`");
                    // Wait while application pool operation is completed
                    if (appPool.State == ObjectState.Stopping || appPool.State == ObjectState.Starting)
                    {
                        WaitOperationCompleted(appPool);
                    }

                    //The app pool is running, so stop it.
                    if (appPool.State == ObjectState.Started)
                    {
                        _logger.WriteDebug($"Application pool `{applicationPoolName}` is started. Stop it.");

                        appPool.Stop();
                        WaitOperationCompleted(appPool);
                        _logger.WriteVerbose($"Application pool `{applicationPoolName}` stopped.");
                    }

                    //The app pool is already stopped.
                    if (appPool.State == ObjectState.Stopped)
                    {
                        _logger.WriteDebug($"Application pool `{applicationPoolName}` is already stopped.");
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
    }
}
