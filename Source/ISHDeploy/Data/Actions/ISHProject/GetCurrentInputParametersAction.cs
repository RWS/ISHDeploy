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
using System.IO;
using System.Linq;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.ISHProject
{
    /// <summary>
    /// Gets all instances of the installed Content Manager deployment for the current system.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class GetCurrentInputParametersAction : BaseActionWithResult<InputParameters>
    {
        /// <summary>
        /// The input parameters file name
        /// </summary>
        private const string InputParametersFileName = "inputparameters.xml";

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
        /// The Content Manager deployment name.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCurrentInputParametersAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="name">The Content Manager deployment name.</param>
        /// <param name="returnResult">The delegate that returns list of Content Manager deployments.</param>
        public GetCurrentInputParametersAction(ILogger logger, string name, Action<InputParameters> returnResult)
            : base(logger, returnResult)
        {
            _registryManager = ObjectFactory.GetInstance<IRegistryManager>();
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _name = name;
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>Content Manager deployment in acccordance with name.</returns>
        protected override InputParameters ExecuteWithResult()
        {
            // Get installed deployment from the registry.
            var projectRegKey = _registryManager.GetInstalledProjectsKeys(_name).FirstOrDefault();
            var installParamsPath = _registryManager.GetInstallParamFilePath(projectRegKey);

            if (string.IsNullOrWhiteSpace(installParamsPath))
            {
                Logger.WriteError(new CorruptedInstallationException($"Registry subkeys for {projectRegKey} are corrupted"), projectRegKey);
            }

            var installParamFile = Path.Combine(installParamsPath, InputParametersFileName);

            if (!_fileManager.FileExists(installParamFile))
            {
                Logger.WriteError(new CorruptedInstallationException($"{ installParamFile } file does not exist on the system"), installParamFile);
            }

            var dictionary = _xmlConfigManager.GetAllInputParamsValues(installParamFile);

            return new InputParameters(installParamFile, dictionary);
        }
    }
}
