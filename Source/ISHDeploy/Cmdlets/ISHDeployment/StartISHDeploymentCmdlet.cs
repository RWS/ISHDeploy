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
using ISHDeploy.Business.Operations.ISHDeployment;

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Starts and enables components of specific Content Manager deployment that marked as "Enabled" in tracking file.</para>
    /// <para type="description">The Start-ISHDeployment cmdlet starts and enables components of specific Content Manager deployment that marked as "Enabled" in tracking file.</para>
    /// <para type="link">Stop-ISHDeployment</para>
    /// <para type="link">Restart-ISHDeployment</para>
    /// <para type="link">Enable-ISHCOMPlus</para>
    /// <para type="link">Enable-ISHIISAppPool</para>
    /// <para type="link">Enable-ISHServiceTranslationBuilder</para>
    /// <para type="link">Enable-ISHServiceTranslationOrganizer</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Start-ISHDeployment -ISHDeployment $deployment</code>
    /// <para>This command enables components of specific Content Manager deployment that marked as "Enabled" in tracking file.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Start, "ISHDeployment")]
    public class StartISHDeploymentCmdlet : BaseISHDeploymentAdminRightsCmdlet
    {
        /// <summary>
        /// Executes revert changes cmdLet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new StartISHDeploymentOperation(Logger, ISHDeployment);

            operation.Run();
        }
    }
}
