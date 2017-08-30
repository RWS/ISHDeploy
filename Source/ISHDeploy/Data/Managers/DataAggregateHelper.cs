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
using System.IO;
using System.Linq;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Exceptions;
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
﻿using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.Backup;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Aggregates data from different places.
    /// </summary>
    /// <seealso cref="IDataAggregateHelper" />
    public class DataAggregateHelper : IDataAggregateHelper
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The registry manager.
        /// </summary>
        private readonly ITrisoftRegistryManager _registryManager;

        /// <summary>
        /// The XML configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The web administration manager.
        /// </summary>
        private readonly IWebAdministrationManager _webAdministrationManage;

        /// <summary>
        /// The COM+ component manager.
        /// </summary>
        private readonly ICOMPlusComponentManager _comPlusComponentManager;

        /// <summary>
        /// The windows service manager.
        /// </summary>
        private readonly IWindowsServiceManager _windowsServiceManager;


        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DataAggregateHelper(ILogger logger)
        {
            _logger = logger;
            _registryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _webAdministrationManage = ObjectFactory.GetInstance<IWebAdministrationManager>();
            _comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
            _windowsServiceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
        }

        /// <summary>
        /// Returns input parameters
        /// </summary>
        /// <param name="deploymentName">The Content Manager deployment name.</param>
        /// <returns>InputParameters containing all parameters from InputParameters.xml file for specified deployment</returns>
        public InputParameters GetInputParameters(string deploymentName)
        {
            _logger.WriteDebug("Get input parameters", deploymentName);
            // Get installed deployment from the registry.
            var projectRegKey = _registryManager.GetInstalledProjectsKeys(deploymentName).FirstOrDefault();
            var installParamsPath = _registryManager.GetInstallParamFilePath(projectRegKey);

            if (string.IsNullOrWhiteSpace(installParamsPath))
            {
                _logger.WriteError(new CorruptedInstallationException($"Registry subkeys for {projectRegKey} are corrupted"), projectRegKey);
            }

            var installParamFile = Path.Combine(installParamsPath, "inputparameters.xml");

            if (!_fileManager.FileExists(installParamFile))
            {
                _logger.WriteError(new CorruptedInstallationException($"{ installParamFile } file does not exist on the system"), installParamFile);
            }

            var dictionary = _xmlConfigManager.GetAllInputParamsValues(installParamFile);

            var inputParameters = new InputParameters(installParamFile, dictionary);

            _logger.WriteVerbose($"Input parameters for `{deploymentName}` deployment has been got");

            return inputParameters;
        }

        /// <summary>
        /// Returns all components of deployment
        /// </summary>
        /// <param name="deploymentName">The Content Manager deployment name.</param>
        /// <returns>The collection of components for specified deployment</returns>
        public ISHComponentsCollection GetComponents(string deploymentName)
        {
            _logger.WriteDebug("Get components and their states", deploymentName);
            var components = new ISHComponentsCollection(true);
            var backgroundTaskServices = _windowsServiceManager.GetISHBackgroundTaskWindowsServices(deploymentName);
            foreach (var backgroundTaskService in backgroundTaskServices)
            {
                if (!components.Components.Any(x => string.Equals(x.Role, backgroundTaskService.Role, StringComparison.CurrentCultureIgnoreCase)))
                {
                    components.Components.Add(new ISHComponent { Name = ISHComponentName.BackgroundTask, Role = backgroundTaskService.Role });
                }
            }
            components.Components.RemoveAll(
                x => x.Name == ISHComponentName.BackgroundTask &&
                    !backgroundTaskServices.Any(
                        bt => string.Equals(bt.Role, x.Role, StringComparison.CurrentCultureIgnoreCase)));

            var inputParameters = GetInputParameters(deploymentName);
            var serviceName = string.Empty;
            foreach (var component in components)
            {
                switch (component.Name)
                {
                    case ISHComponentName.CM:
                        component.IsEnabled = _webAdministrationManage.IsApplicationPoolStarted(inputParameters.CMAppPoolName);
                        break;
                    case ISHComponentName.WS:
                        component.IsEnabled = _webAdministrationManage.IsApplicationPoolStarted(inputParameters.WSAppPoolName);
                        break;
                    case ISHComponentName.STS:
                        component.IsEnabled = _webAdministrationManage.IsApplicationPoolStarted(inputParameters.STSAppPoolName);
                        break;
                    case ISHComponentName.COMPlus:
                        component.IsEnabled = _comPlusComponentManager.IsCOMPlusComponentEnabled("Trisoft-InfoShare-Author");
                        break;
                    case ISHComponentName.TranslationOrganizer:
                        serviceName = _windowsServiceManager.GetServices(deploymentName, ISHWindowsServiceType.TranslationOrganizer).First().Name;
                        component.IsEnabled = _windowsServiceManager.IsWindowsServiceStarted(serviceName);
                        break;
                    case ISHComponentName.TranslationBuilder:
                        serviceName = _windowsServiceManager.GetServices(deploymentName, ISHWindowsServiceType.TranslationBuilder).First().Name;
                        component.IsEnabled = _windowsServiceManager.IsWindowsServiceStarted(serviceName);
                        break;
                    case ISHComponentName.Crawler:
                        serviceName = _windowsServiceManager.GetServices(deploymentName, ISHWindowsServiceType.Crawler).First().Name;
                        component.IsEnabled = _windowsServiceManager.IsWindowsServiceStarted(serviceName);
                        break;
                    case ISHComponentName.BackgroundTask:
                        serviceName = backgroundTaskServices.First(x => string.Equals(x.Role, component.Role, StringComparison.CurrentCultureIgnoreCase)).Name;
                        component.IsEnabled = _windowsServiceManager.IsWindowsServiceStarted(serviceName);
                        break;
                    case ISHComponentName.SolrLucene:
                        serviceName = _windowsServiceManager.GetServices(deploymentName, ISHWindowsServiceType.SolrLucene).Single().Name;
                        component.IsEnabled = _windowsServiceManager.IsWindowsServiceStarted(serviceName);
                        break;
                }
            }

            return components;
        }

        /// <summary>
        /// Save all components of deployment
        /// </summary>
        /// <param name="filePath">The path to file.</param>
        /// <param name="collection">The collection of components for specified deployment</param>
        public void SaveComponents(string filePath, ISHComponentsCollection collection)
        {
            _xmlConfigManager.SerializeToFile(filePath, collection);
        }

        /// <summary>
        /// Returns all components of deployment which were saved in a file 
        /// </summary>
        /// <param name="filePath">The path to file.</param>
        /// <returns>The collection of components readed from file</returns>
        public ISHComponentsCollection ReadComponentsFromFile(string filePath)
        {
            if (!_fileManager.FileExists(filePath))
            {
                return new ISHComponentsCollection(true);
            }
            return _xmlConfigManager.Deserialize<ISHComponentsCollection>(filePath);
        }

        /// <summary>
        /// Returns all windows services with all properties needed for their recreation
        /// </summary>
        /// <param name="deploymentName">The name of deployment.</param>
        /// <returns>The collection of windows services with all properties needed for their recreation</returns>
        public ISHWindowsServiceBackupCollection GetISHWindowsServiceBackupCollection(string deploymentName)
        {
            var services = _windowsServiceManager.GetAllServices(deploymentName);
            var backup = new ISHWindowsServiceBackupCollection();
            foreach (var service in services)
            {
                var registryPath = $@"SYSTEM\CurrentControlSet\Services\{service.Name}";
                var namesOfValues = _registryManager.GetValueNames(registryPath);
                backup.Services.Add(new ISHWindowsServiceBackup
                {
                    Name = service.Name,
                    WindowsServiceManagerProperties = _windowsServiceManager.GetWindowsServiceProperties(service.Name),
                    RegistryManagerProperties = _registryManager.GetValues(namesOfValues, registryPath)
                });
            }

            return backup;
        }
    }
}
