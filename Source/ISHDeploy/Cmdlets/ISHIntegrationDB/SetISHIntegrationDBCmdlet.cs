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
using ISHDeploy.Business.Operations.ISHIntegrationDB;
using System.Management.Automation;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHIntegrationDB
{
    /// <summary>
    /// <para type="synopsis">Change connection string to database for certain environment.</para>
    /// <para type="description">This cmdlet change connection string to database for certain environment.</para>
    /// </summary>
    /// <seealso cref="BaseHistoryEntryCmdlet" />
    /// <example>
    ///   <code>PS C:\&gt;Set-ISHIntegrationDB -ISHDeployment $deployment -ConnectionString ""</code>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationDB")]
    public class SetISHIntegrationDBCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Connection string.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Connection string.", ParameterSetName = "ConnectionString")]
        [Parameter(Mandatory = true, HelpMessage = "The type of database.", ParameterSetName = "ConnectionStringBuilder")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// <para type="description">The type of database.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The type of database.", ParameterSetName = "ConnectionStringBuilder")]
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// <para type="description">Set connection string as a string parameter.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "When -Raw is switched on then connection string will be set as a string parameter.", ParameterSetName = "ConnectionString")]
        public SwitchParameter Raw { get; set; }

        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (ParameterSetName == "ConnectionString")
            {
                var operation = new SetISHIntegrationDBOperation(Logger, ISHDeployment, ConnectionString, DatabaseType);
                operation.Run();
            }
        }
    }
}
