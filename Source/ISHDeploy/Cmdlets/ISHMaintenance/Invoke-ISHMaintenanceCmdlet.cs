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
using ISHDeploy.Common.Enums;
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
    /// <example>
    /// <code>PS C:\>Invoke-ISHMaintenance -ISHDeployment $deployment -Crawler -UnRegisterAll -CrawlerTridkApp "InfoShareBuilders"</code>
    /// <para>This command unregisters the Crawler for 'TrisoftInfoShareIndex' for specified deployment.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Invoke-ISHMaintenance -ISHDeployment $deployment -Crawler -ReIndex -CrawlerTridkApp "InfoShareBuilders"</code>
    /// <para>This command does reindex of the Crawler for 'TrisoftInfoShareIndex' for specified deployment.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Invoke-ISHMaintenance -ISHDeployment $deployment -FullTextIndex -Cleanup</code>
    /// <para>This command cleans up all cache and temporary files of Crawler for specified deployment.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Invoke, "ISHMaintenance")]
    public sealed class InvokeISHMaintenanceCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Maintenance the Crawler service. Registering the Crawler for 'TrisoftInfoShareIndex' on 'UADEVVMASKYMENK'.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Register the Crawler", ParameterSetName = "Register")]
        [Parameter(Mandatory = true, HelpMessage = "Unregister the Crawler", ParameterSetName = "UnRegisterAll")]
        [Parameter(Mandatory = true, HelpMessage = "Reindex the Crawler", ParameterSetName = "ReIndex")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Crawler { get; set; }

        /// <summary>
        /// <para type="description">Register the Crawler.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Register the Crawler", ParameterSetName = "Register")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Register { get; set; }

        /// <summary>
        /// <para type="description">Unregister the Crawler.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Unregister the Crawler", ParameterSetName = "UnRegisterAll")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter UnRegisterAll { get; set; }

        /// <summary>
        /// <para type="description">Reindex the Crawler.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Reindex the Crawler", ParameterSetName = "ReIndex")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter ReIndex { get; set; }

        /// <summary>
        /// <para type="description">The ReIndex card type.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The ReIndex card type", ParameterSetName = "ReIndex")]
        [ValidateNotNullOrEmpty]
        public string ReIndexCardType { get; set; } = "ISHAll";

        /// <summary>
        /// <para type="description">Cleanup FullTextIndex.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Cleanup FullTextIndex", ParameterSetName = "CleanupSolrLucene")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter FullTextIndex { get; set; }

        /// <summary>
        /// <para type="description">Cleanup FullTextIndex.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Cleanup FullTextIndex", ParameterSetName = "CleanupSolrLucene")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Cleanup { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            IOperation operation = null;
            switch (ParameterSetName)
            {
                case "Register":
                    operation = new InvokeISHMaintenanceCrawlerOperation(Logger, ISHDeployment, RegisterCrawlerOperationType.register);
                    break;
                case "UnRegisterAll":
                    operation = new InvokeISHMaintenanceCrawlerOperation(Logger, ISHDeployment, RegisterCrawlerOperationType.unregister);
                    break;
                case "ReIndex":
                    operation = new InvokeISHMaintenanceCrawlerOperation(Logger, ISHDeployment, ReIndexCardType);
                    break;
                case "CleanupSolrLucene":
                    operation = new InvokeISHMaintenanceCleanUpSolrLuceneOperation(Logger, ISHDeployment);
                    break;
            }

            operation.Run();
        }
    }
}
