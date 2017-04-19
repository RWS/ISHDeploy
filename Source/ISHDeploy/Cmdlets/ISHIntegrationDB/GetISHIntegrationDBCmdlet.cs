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
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Cmdlets.ISHIntegrationDB
{
    /// <summary>
    /// <para type="synopsis">Return connection string to database for certain environment.</para>
    /// <para type="description">This cmdlet returns connection string to database for certain environment.</para>
    /// </summary>
    /// <seealso cref="BaseHistoryEntryCmdlet" />
    /// <example>
    ///   <code>PS C:\&gt;Get-ISHIntegrationDB -ISHDeployment $deployment</code>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHIntegrationDB")]
    public class GetISHIntegrationDbCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var trisoftRegistryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            var value = trisoftRegistryManager.GetRegistryValue(RegistryValueName.DbConnectionString, ISHDeployment.WebAppNameCM);

            ISHWriteOutput(value);
        }
    }
}
