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
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationDB
{
    /// <summary>
    /// Gets connection string to database and type of database.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHIntegrationDBOperation : BaseOperationPaths, IOperation<IntegrationDbConnectionString>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHIntegrationDBOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public GetISHIntegrationDBOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment)
            : base(logger, ishDeployment)
        {
            
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public IntegrationDbConnectionString Run()
        {
            var trisoftRegistryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();

            var connectionString = (string)trisoftRegistryManager.GetRegistryValue(RegistryValueName.DbConnectionString, $"InfoShareAuthor{InputParameters.ProjectSuffix}");
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), trisoftRegistryManager.GetRegistryValue(RegistryValueName.DatabaseType, $"InfoShareAuthor{InputParameters.ProjectSuffix}").ToString());

            return new IntegrationDbConnectionString(connectionString, databaseType);
        }
    }
}
