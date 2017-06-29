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
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHComponent;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHMaintenance
{
    /// <summary>
    /// <para type="synopsis">Invokes settings of Crawler services.</para>
    /// <para type="description">The Invoke-ISHMaintenance cmdlet sets settings of Crawler services.</para>
    /// <para type="link">Enable-ISHMaintenance</para>
    /// <para type="link">Disable-ISHMaintenance</para>
    /// <para type="link">Get-ISHMaintenance</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Invoke-ISHMaintenance -ISHDeployment $deployment -Count 2</code>
    /// <para>This command changes the amount of instances of Crawler services.
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
            if (!MyInvocation.BoundParameters.ContainsKey("Count") &&
                !MyInvocation.BoundParameters.ContainsKey("Hostname") &&
                !MyInvocation.BoundParameters.ContainsKey("Catalog"))
            {
                throw new ArgumentException("Invoke-ISHMaintenance cmdlet has no parameters to set");
            }


           


        }
    }
}
