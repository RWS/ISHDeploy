/**
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
using ISHDeploy.Business.Operations.ISHExternalPreview;

namespace ISHDeploy.Cmdlets.ISHExternalPreview
{
    /// <summary>
    /// <para type="synopsis">Enables external preview for Content Manager deployment for specific user.</para>
    /// <para type="description">The Enable-ISHExternalPreview cmdlet enables external preview for Content Manager deployment for specific user.</para>
    /// <para type="description">If user id is not specified, the default value 'ServiceUser' is taken.</para>
    /// <para type="link">Disable-ISHExternalPreview</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Enable-ISHExternalPreview -ISHDeployment $deployment -ExternalId 'user1'</code>
    /// <para>This command enables the external preview for user 'user1'.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHExternalPreview")]
    public sealed class EnableISHExternalPreviewCmdlet : BaseHistoryEntryCmdlet
    {

        /// <summary>
        /// <para type="description">External user id for which external preview will be enabled. Default value is ServiceUser.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string ExternalId { get; set; } = "ServiceUser";

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new EnableISHExternalPreviewOperation(Logger, ISHDeployment, ExternalId);

            operation.Run();
        }
    }
}
