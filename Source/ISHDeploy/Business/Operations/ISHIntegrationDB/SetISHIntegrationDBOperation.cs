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
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.Registry;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.XmlFile;

namespace ISHDeploy.Business.Operations.ISHIntegrationDB
{
    /// <summary>
    /// Set connection string to database.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIntegrationDBOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationDBOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="connectionString">Connection string to check.</param>
        /// <param name="databaseType">The type of Database.</param>
        public SetISHIntegrationDBOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, string connectionString, DatabaseType databaseType)
             :base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Set connection to database with {connectionString} connection string.");

            // change registry
            Invoker.AddAction(new SetRegistryValueAction(logger, RegistryValueName.DbConnectionString, connectionString, $"InfoShareAuthor{InputParameters.ProjectSuffix}"));
            Invoker.AddAction(new SetRegistryValueAction(logger, RegistryValueName.DatabaseType, databaseType, $"InfoShareAuthor{InputParameters.ProjectSuffix}"));
            Invoker.AddAction(new SetRegistryValueAction(logger, RegistryValueName.DbConnectionString, connectionString, $"InfoShareBuilders{InputParameters.ProjectSuffix}"));
            Invoker.AddAction(new SetRegistryValueAction(logger, RegistryValueName.DatabaseType, databaseType, $"InfoShareBuilders{InputParameters.ProjectSuffix}"));

            // change inputparameters.xml
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ConnectionStringXPath, connectionString));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.DatabaseTypeXPath, databaseType.ToString()));

            // restart services
            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.WSAppPoolName, true));
            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName, true));
            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.CMAppPoolName, true));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }
    }
}
