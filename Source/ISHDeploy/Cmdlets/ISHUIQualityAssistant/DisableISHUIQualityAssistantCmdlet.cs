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
using ISHDeploy.Business.Operations.ISHUIQualityAssistant;

namespace ISHDeploy.Cmdlets.ISHUIQualityAssistant
{
    /// <summary>
    /// <para type="synopsis">Disables Quality Assistant for Content Manager deployment.</para>
    /// <para type="description">The Disable-ISHUIQualityAssistant cmdlet disables Quality Assistant for Content Manager deployment.</para>
    /// <para type="link">Enable-ISHUIQualityAssistant</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Disable-ISHUIQualityAssistant -ISHDeployment $deployment</code>
    /// <para>This command disables Quality Assistant.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIQualityAssistant")]
    [AdministratorRights]
    public sealed class DisableISHUIQualityAssistantCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHUIQualityAssistantOperation(Logger, ISHDeployment);
            
            operation.Run();
        }
    }
}
