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

namespace ISHDeploy.Cmdlets.ISHServiceCrawler
{
    /// <summary>
    /// <para type="synopsis">Sets settings of Crawler services.</para>
    /// <para type="description">The Set-ISHServiceCrawler cmdlet sets settings of Crawler services.</para>
    /// <para type="link">Enable-ISHServiceCrawler</para>
    /// <para type="link">Disable-ISHServiceCrawler</para>
    /// <para type="link">Get-ISHServiceCrawler</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceCrawler -ISHDeployment $deployment -Count 2</code>
    /// <para>This command changes the amount of instances of Crawler services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceCrawler -ISHDeployment $deployment -Hostname "testmachinename"</code>
    /// <para>This command changes the hostname of instances of Crawler services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceCrawler -ISHDeployment $deployment -Catalog "TrisoftInfoShareIndex"</code>
    /// <para>This command changes the catalog name of instances of Crawler services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHServiceCrawler")]
    public sealed class SetISHServiceCrawlerCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The number of Crawler services in the system.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The number of Crawler services in the system")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 10)]
        public int Count { get; set; }

        /// <summary>
        /// <para type="description">The Hostname of Crawler service.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The Hostname of Crawler service")]
        [ValidateNotNullOrEmpty]
        public string Hostname { get; set; }

        /// <summary>
        /// <para type="description">The Catalog  of Crawler service.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The Catalog of Crawler service")]
        [ValidateNotNullOrEmpty]
        public string Catalog { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (!MyInvocation.BoundParameters.ContainsKey("Count") &&
                !MyInvocation.BoundParameters.ContainsKey("Hostname") &&
                !MyInvocation.BoundParameters.ContainsKey("Catalog"))
            {
                throw new ArgumentException("Set-ISHServiceCrawler cmdlet has no parameters to set");
            }


            if (MyInvocation.BoundParameters.ContainsKey("Count"))
            {
                var operation = new SetISHServiceAmountOperation(Logger, ISHDeployment, Count, ISHWindowsServiceType.Crawler);

                operation.Run();
            }

            if (MyInvocation.BoundParameters.ContainsKey("Hostname"))
            {
                var operation = new SetISHBuildersRegistryValueOperation(Logger, ISHDeployment, RegistryValueName.CrawlerCatalogHostName, Hostname);

                operation.Run();
            }

            if (MyInvocation.BoundParameters.ContainsKey("Catalog"))
            {
                var operation = new SetISHBuildersRegistryValueOperation(Logger, ISHDeployment, RegistryValueName.CrawlerCatalogName, Catalog);

                operation.Run();
            }


        }
    }
}
