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
using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Acquire the components of a specific deployment.</para>
    /// <para type="description">The Get-ISHComponent cmdlet gets components for Content Manager deployment.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHComponent -ISHDeployment $deployment</code>
    /// <para>This command gets list of components of Content Manager deployment
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHComponent")]
    public sealed class GetISHComponentCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new GetISHComponentOperation(Logger, ISHDeployment);

            ISHWriteOutput(operation.Run());
        }
    }
}
