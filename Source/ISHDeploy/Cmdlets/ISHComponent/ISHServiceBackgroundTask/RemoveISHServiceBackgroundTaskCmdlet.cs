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

namespace ISHDeploy.Cmdlets.ISHComponent.ISHServiceBackgroundTask
{
    /// <summary>
    /// <para type="synopsis">Removes BackgroundTask windows service.</para>
    /// <para type="description">The Remove-ISHServiceBackgroundTask cmdlet removes all BackgroundTask windows services of specified role.</para>
    /// <para type="link">Enable-ISHServiceBackgroundTask</para>
    /// <para type="link">Disable-ISHServiceBackgroundTask</para>
    /// <para type="link">Get-ISHServiceBackgroundTask</para>
    /// <para type="link">Set-ISHServiceBackgroundTask</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Remove-ISHServiceBackgroundTask -ISHDeployment $deployment</code>
    /// <para>This command removes all instances of BackgroundTask windows services of Default role.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Remove-ISHServiceBackgroundTask -ISHDeployment $deployment -Role "PublishOnly"</code>
    /// <para>This command removes all instances of BackgroundTask windows services with role "PublishOnly".
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "ISHServiceBackgroundTask")]
    public sealed class RemoveISHServiceBackgroundTaskCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The role of BackgroundTask services.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "The BackgroundTask role")]
        [ValidateNotNullOrEmpty]
        public string Role { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new SetISHServiceBackgroundTaskAmountOperation(Logger, ISHDeployment, 0, Role);

            operation.Run();
        }
    }
}
