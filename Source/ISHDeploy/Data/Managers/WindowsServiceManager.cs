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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using System.ServiceProcess;

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
        /// Initializes a new instance of the <see cref="WindowsServiceManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public WindowsServiceManager(ILogger logger)
        {
            _logger = logger;
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
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    _logger.WriteVerbose($"Windows service `{serviceName}` has been started");
                }
                else
                {
                    _logger.WriteVerbose($"Windows service `{serviceName}` has already started");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.InnerException);
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
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    _logger.WriteVerbose($"Windows service `{serviceName}` has been stopped");
                }
                else
                {
                    _logger.WriteVerbose($"Windows service `{serviceName}` has already stopped");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.InnerException);
            }
        }

        /// <summary>
        /// Gets names of all windows services where name contains search criteria.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns>
        /// The names of all windows services where name contains search criteria.
        /// </returns>
        public IEnumerable<string> GetNamesOfWindowsServicesWhereNameContains(string searchCriteria)
        {
            return
                ServiceController.GetServices()
                    .Where(x => x.ServiceName.Contains(searchCriteria))
                    .Select(x => x.ServiceName);
        }

        /// <summary>
        /// Gets current Status of the service.
        /// </summary>
        /// <param name="serviceName">Name of the windows service.</param>
        /// <returns>
        /// The current Status of the service.
        /// </returns>
        public ServiceControllerStatus GetWindowsServiceStatus(string serviceName)
        {
            _logger.WriteDebug("Get Windows service status", serviceName);
            var service = new ServiceController(serviceName);
            var status = service.Status;
            _logger.WriteVerbose($"Windows service `{serviceName}` has status `{status}`");
            return status;
        }
    }
}
