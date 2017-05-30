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
using ISHDeploy.Business.Operations.ISHComponent;
using ISHDeploy.Common.Enums;

namespace ISHDeploy.Cmdlets.ISHServiceBackgroundTask
{
    /// <summary>
    /// <para type="synopsis">Sets BackgroundTask windows service.</para>
    /// <para type="description">The Set-ISHServiceBackgroundTask cmdlet sets BackgroundTask windows service.</para>
    /// <para type="link">Enable-ISHExternalPreview</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHServiceBackgroundTask -ISHDeployment $deployment -Count 2</code>
    /// <para>This command changes the amount of instances of services.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHServiceBackgroundTask")]
    public sealed class SetISHServiceBackgroundTaskCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The number of BackgroundTask services in the system.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The number of BackgroundTask services in the system", ParameterSetName = "BackgroundTaskCount")]
        [ValidateNotNullOrEmpty]
        [ValidateRange(1, 10)]
        public int Count { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (ParameterSetName == "BackgroundTaskCount")
            {
                var operation = new SetISHServiceAmountOperation(Logger, ISHDeployment, Count, ISHWindowsServiceType.BackgroundTask);

                operation.Run();
            }
        }
    }
}
