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
using System.Collections.Generic;
﻿using System.IO;
﻿using System.Linq;
﻿using ISHDeploy.Common;
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.ISHProject
{
    /// <summary>
    /// Gets all instances of the installed Content Manager deployment for the current system.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class GetISHDeploymentsAction : BaseActionWithResult<IEnumerable<ISHDeployment>>
    {
        /// <summary>
        /// The aggregator of data.
        /// </summary>
        private readonly IDataAggregateHelper _dataAggregateHelper;

        /// <summary>
        /// The registry manager.
        /// </summary>
        private readonly ITrisoftRegistryManager _registryManager;

        /// <summary>
        /// The Content Manager deployment name.
        /// </summary>
        private readonly string _projectName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="projectName">The Content Manager deployment name.</param>
        /// <param name="returnResult">The delegate that returns list of Content Manager deployments.</param>
        public GetISHDeploymentsAction(ILogger logger, string projectName, Action<IEnumerable<ISHDeployment>> returnResult)
            : base(logger, returnResult)
        {
            _dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            _registryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            _projectName = projectName;
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>Content Manager deployment in acccordance with name.</returns>
        protected override IEnumerable<ISHDeployment> ExecuteWithResult()
        {
            var result = new List<ISHDeployment>();

            // Get list of installed deployments from the registry.
            var installProjectsRegKeys = _registryManager.GetInstalledProjectsKeys(_projectName).ToArray();

            if (!installProjectsRegKeys.Any())
            {
                if (!string.IsNullOrEmpty(_projectName))
                {
                    throw new DeploymentNotFoundException($"Deployment with name {_projectName} is not found on the system");
                }
                else
                {
                    Logger.WriteVerbose("None project instances were found on the system");
                    return result;
                }
            }

            // For each registry record get deployment version, path to inputparameter.xml file and parse this file.
            foreach (var projectRegKey in installProjectsRegKeys)
            {
                var version = _registryManager.GetInstalledProjectVersion(projectRegKey);
                var parameters = _dataAggregateHelper.GetInputParameters(projectRegKey.Name.Split('\\').Last());
                var ishProject = new ISHDeployment
                {
                    Name = $"InfoShare{parameters.ProjectSuffix}",
                    AppPath = Path.Combine(parameters.AppPath, $"App{parameters.ProjectSuffix}"),
                    WebPath = Path.Combine(parameters.WebPath, $"Web{parameters.ProjectSuffix}"),
                    DataPath = Path.Combine(parameters.DataPath, $"Data{parameters.ProjectSuffix}"),
                    DatabaseType = parameters.DatabaseType,
                    AccessHostName = parameters.AccessHostName,
                    WebAppNameCM = parameters.WebAppNameCM,
                    WebAppNameWS = parameters.WebAppNameWS,
                    WebAppNameSTS = parameters.WebAppNameSTS,
                    WebSiteName = parameters.WebSiteName,
                    SoftwareVersion = version
                };

                ishProject.Status = _dataAggregateHelper.GetISHDeploymentStatus(ishProject.Name);

                result.Add(ishProject);
            }

            return result;
        }
    }
}
