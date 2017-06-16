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
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationDB
{
    /// <summary>
    /// Tests connection string to database
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class TestISHIntegrationDBOperation : BaseOperationPaths, IOperation<bool>
    {
        /// <summary>
        /// The connection string to database
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHIntegrationDBOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="connectionString">The connection string to database.</param>
        public TestISHIntegrationDBOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, string connectionString)
            : base(logger, ishDeployment)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public bool Run()
        {
            var trisoftRegistryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            var databaseManager = ObjectFactory.GetInstance<IDatabaseManager>();


            // Get ConnectionString and DatabaseType for InfoShareAuthor and InfoShareBuilders
            var infoShareAuthorConnectionString = trisoftRegistryManager.GetRegistryValue(InfoShareAuthorRegistryElement, RegistryValueName.Connect.ToString()).ToString().ToLower();
            var infoShareAuthorDatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), trisoftRegistryManager.GetRegistryValue(InfoShareAuthorRegistryElement, RegistryValueName.ComponentName.ToString()).ToString());

            var infoShareBuildersConnectionString = trisoftRegistryManager.GetRegistryValue(InfoShareBuildersRegistryElement, RegistryValueName.Connect.ToString()).ToString().ToLower();
            var infoShareBuildersDatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), trisoftRegistryManager.GetRegistryValue(InfoShareBuildersRegistryElement, RegistryValueName.ComponentName.ToString()).ToString());

            // Compare values
            bool isConnectionStringValid = infoShareAuthorConnectionString == infoShareBuildersConnectionString;
            if (!isConnectionStringValid)
            {
                Logger.WriteWarning($"The value `{RegistryValueName.Connect}` for `{InfoShareAuthorRegistryElement}` is not the same as for `{InfoShareBuildersRegistryElement}`");
            }

            isConnectionStringValid = isConnectionStringValid && infoShareAuthorDatabaseType == infoShareBuildersDatabaseType;
            if (!isConnectionStringValid)
            {
                Logger.WriteWarning($"The value `{RegistryValueName.ComponentName}` for `{InfoShareAuthorRegistryElement}` is not the same as for `{InfoShareBuildersRegistryElement}`");
            }

            isConnectionStringValid = isConnectionStringValid && databaseManager.TestConnection(_connectionString);
            return isConnectionStringValid;
        }
    }
}
