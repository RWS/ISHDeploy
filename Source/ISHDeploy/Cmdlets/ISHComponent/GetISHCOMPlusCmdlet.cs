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
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Cmdlets.ISHComponent
{
    /// <summary>
    /// <para type="synopsis">Gets list of COM+ components.</para>
    /// <para type="description">The Get-ISHCOMPlus cmdlet gets list of COM+ components.</para>
    /// <para type="link">Enable-ISHCOMPlus</para>
    /// <para type="link">Disable-ISHCOMPlus</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHCOMPlus -ISHDeployment $deployment</code>
    /// <para>This command shows list of COM+ components.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHCOMPlus")]
    public sealed class GetISHCOMPlusCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var comPlusComponentManager = ObjectFactory.GetInstance<ICOMPlusComponentManager>();
            var comPlusComponents = comPlusComponentManager.GetCOMPlusComponents();

            ISHWriteOutput(comPlusComponents);
        }
    }
}
