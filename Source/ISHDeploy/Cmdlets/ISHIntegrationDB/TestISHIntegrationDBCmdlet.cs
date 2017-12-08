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
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHIntegrationDB
{
    /// <summary>
    /// <para type="synopsis">Test connection string to database.</para>
    /// <para type="description">This cmdlet check connection to database for certain environment and returns True if connection is Ok, or False if connection is invalid</para>
    /// <para type="description">This cmdlet doesn't support check of connection if type of database of environment is Oracle.</para>
    /// </summary>
    /// <seealso cref="BaseHistoryEntryCmdlet" />
    /// <example>
    ///   <code>PS C:\&gt;Test-ISHIntegrationDB -ISHDeployment $deployment</code>
    /// <para>This cmdlet tests connection to database for certain environment and returns True if connection is Ok, or False if connection is invalid. This command doesn't support check of connection to Oracle database.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsDiagnostic.Test, "ISHIntegrationDB")]
    public class TestISHIntegrationDBCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var connectionString = new GetISHIntegrationDBOperation(Logger, ISHDeployment).Run();

            if (connectionString.Engine == DatabaseType.oracle)
            {
                WriteWarning("Connection check doesn't support Oracle database");
            }
            else
            {
                ISHWriteOutput(new TestISHIntegrationDBOperation(Logger, ISHDeployment, connectionString.RawConnectionString).Run());
            }
        }
    }
}
