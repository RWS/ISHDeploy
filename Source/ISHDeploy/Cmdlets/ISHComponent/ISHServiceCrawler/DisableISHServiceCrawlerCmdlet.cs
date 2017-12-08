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
﻿using System.Management.Automation;
﻿using ISHDeploy.Business.Operations.ISHComponent;
﻿using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHComponent.ISHServiceCrawler
{
    /// <summary>
    /// <para type="synopsis">Disables Crawler windows service.</para>
    /// <para type="description">The Disable-ISHServiceCrawler cmdlet disables Crawler windows service.</para>
    /// <para type="link">Enable-ISHServiceCrawler</para>
    /// <para type="link">Set-ISHServiceCrawler</para>
    /// <para type="link">Get-ISHServiceCrawler</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Disable-ISHServiceCrawler -ISHDeployment $deployment</code>
    /// <para>This command disables Crawler windows service.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Disable, "ISHServiceCrawler")]
    public sealed class DisableISHServiceCrawlerBuilderCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHComponentOperation(Logger, ISHDeployment, ISHComponentName.Crawler);

            operation.Run();
        }
    }
}
