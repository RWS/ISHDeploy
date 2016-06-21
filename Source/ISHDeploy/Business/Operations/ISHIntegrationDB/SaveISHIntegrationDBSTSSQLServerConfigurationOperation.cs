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
﻿using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationDB
{
    /// <summary>
    /// Generates SQL Server configuration script that grants necessary permissions.
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class SaveISHIntegrationDBSTSSQLServerConfigurationOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveISHIntegrationDBSTSSQLServerConfigurationOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="fileName">Name of the output file.</param>
        /// <param name="type">The output file type.</param>
        public SaveISHIntegrationDBSTSSQLServerConfigurationOperation(ILogger logger, Models.ISHDeployment ishDeployment, string fileName, OutputType type) :
            base (logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Saving STS integration configuration");

            string templateFile;
            
            switch (type)
            {
                case OutputType.PS1:
                    templateFile = TemporaryDBConfigurationFileNames.GrantComputerAccountPermissionsPSTemplate;
                    break;
                default:
                    templateFile = TemporaryDBConfigurationFileNames.GrantComputerAccountPermissionsSQLTemplate;
                    break;
            }

            _invoker.AddAction(new DirectoryEnsureExistsAction(logger, FoldersPaths.PackagesFolderPath));

            using (OleDbConnection builder = new OleDbConnection(ISHDeploymentInternal.ConnectString))
            {
                /*
                    $principal="GLOBAL\MECDEVASAR02$"   "$OSUSER$"
                    $dbName="ALEXMECULAB1201"           "$DATABASE$"
                    $server="MECDEVDB05\SQL2014SP1"     "$DATASOURCE$"
                 */

                _invoker.AddAction(new FileGenerateFromTemplateAction(logger,
                    templateFile,
                    Path.Combine(FoldersPaths.PackagesFolderPath, fileName),
                    new Dictionary<string, string>
                    {
                        {"$OSUSER$", ISHDeploymentInternal.OSUser},
                        {"$DATABASE$", builder.Database},
                        {"$DATASOURCE$", builder.DataSource}
                    }));
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
