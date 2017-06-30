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
using ISHDeploy.Business.Operations.ISHMaintenance;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Cmdlets.ISHMaintenance
{
    /// <summary>
    /// <para type="synopsis">Maintenance Crawler or FullTextIndex services.</para>
    /// <para type="description">The Invoke-ISHMaintenance cmdlet maintenances Crawler or FullTextIndex services.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Invoke-ISHMaintenance -ISHDeployment $deployment -Crawler -Register -CrawlerTridkApp "InfoShareBuilders"</code>
    /// <para>This command registers the Crawler for 'TrisoftInfoShareIndex' for specified deployment.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Invoke, "ISHMaintenance")]
    public sealed class InvokeISHMaintenanceCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Maintenance the Crawler service.  Registering the Crawler for 'TrisoftInfoShareIndex' on 'UADEVVMASKYMENK'.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Maintenance the Crawler service", ParameterSetName = "Register")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Crawler { get; set; }

        /// <summary>
        /// <para type="description">Register the Crawler.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Register the Crawler", ParameterSetName = "Register")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Register { get; set; }

        /// <summary>
        /// <para type="description">The TridkApp key to registrate the Crawler for it.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The TridkApp key to registrate the Crawler for it", ParameterSetName = "Register")]
        [ValidateNotNullOrEmpty]
        public string CrawlerTridkApp { get; set; } = "InfoShareBuilders";

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            IOperation operation = null;
            switch (ParameterSetName)
            {
                case "Register":
                    operation = new InvokeISHMaintenanceRegisterThisCrawlerOperation(Logger, ISHDeployment, CrawlerTridkApp);
                    break;
            }

            operation.Run();

        }
    }
}
