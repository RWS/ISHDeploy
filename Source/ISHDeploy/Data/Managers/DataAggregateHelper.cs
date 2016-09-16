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
using System.IO;
using System.Linq;
﻿using ISHDeploy.Data.Exceptions;
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
﻿using ISHDeploy.Models;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Aggregates data from different places.
    /// </summary>
    /// <seealso cref="IDataAggregator" />
    public class DataAggregateHelper : IDataAggregateHelper
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The registry manager.
        /// </summary>
        private readonly IRegistryManager _registryManager;

        /// <summary>
        /// The XML configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DataAggregateHelper(ILogger logger)
        {
            _logger = logger;
            _registryManager = ObjectFactory.GetInstance<IRegistryManager>();
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
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
    }
}
