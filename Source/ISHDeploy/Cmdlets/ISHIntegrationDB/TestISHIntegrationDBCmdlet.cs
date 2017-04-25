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

using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHIntegrationDB;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Cmdlets.ISHIntegrationDB
{
    /// <summary>
    /// <para type="synopsis">Test connection string to database.</para>
    /// <para type="description">This cmdlet check connection to database with connection string and returns True if connection is Ok, or False if connection is invalid</para>
    /// </summary>
    /// <seealso cref="BaseCmdlet" />
    /// <example>
    ///   <code>PS C:\&gt;Test-ISHIntegrationDB -ConnectionString "Provider=SQLOLEDB.1;Password=...;Persist Security Info=True;User ID=...;Initial Catalog=...;Data Source=...\SQL2012SP2"</code>
    /// </example>
    [Cmdlet(VerbsDiagnostic.Test, "ISHIntegrationDB")]
    public class TestISHIntegrationDBCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">Connection string.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Connection string.")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var databaseManager = ObjectFactory.GetInstance<IDatabaseManager>();
            if (MyInvocation.BoundParameters.ContainsKey("ConnectionString"))
            {
                ISHWriteOutput(databaseManager.TestConnection(ConnectionString));
            }
            else
            {
                var integrationDbConnectionString = new GetISHIntegrationDBOperation(Logger, ISHDeployment).Run();
                ISHWriteOutput(databaseManager.TestConnection(integrationDbConnectionString.RawConnectionString));
            }
        }
    }
}
