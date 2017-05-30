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

namespace ISHDeploy.Cmdlets.ISHServiceCrawler
{
    /// <summary>
    /// <para type="synopsis">Gets list of windows services for Crawler.</para>
    /// <para type="description">The Get-ISHServiceCrawler cmdlet gets list of Crawler windows services.</para>
    /// <para type="link">Set-ISHServiceCrawler</para>
    /// <para type="link">Enable-ISHServiceCrawler</para>
    /// <para type="link">Disable-ISHServiceCrawler</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHServiceCrawler -ISHDeployment $deployment</code>
    /// <para>This command shows the Crawler windows services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHServiceCrawler")]
    public sealed class GetISHServiceCrawlerCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            var services = serviceManager.GetServices(ISHDeployment.Name, ISHWindowsServiceType.Crawler);

            ISHWriteOutput(services);
        }
    }
}
