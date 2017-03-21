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
using System;
using System.Linq;

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Acquire the input parameters of a specific deployment.</para>
    /// <para type="description">The Get-ISHDeploymentParameters cmdlet gets parameters for Content Manager deployment.</para>
    /// <para type="link">Get-ISHDeployment</para>
    /// <para type="link">Get-ISHDeploymentHistory</para>
    /// <para type="link">Undo-ISHDeployment</para>
    /// <para type="link">Clear-ISHDeploymentHistory</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment $deployment</code>
    /// <para>This command gets the input parameters of Content Manager deployment with hidden passwords.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment $deployment -ShowPassword</code>
    /// <para>This command gets the input parameters of "InfoShare" deployment with real passwords.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment $deployment -Original</code>
    /// <para>This command return original values (from the backup folder) of "InfoShare" deployment.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment $deployment -Changed</code>
    /// <para>This command return only changed parameters of "InfoShare" deployment.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment $deployment -Name "issuercertificatethumbprint"</code>
    /// <para>This command return the parameter with name "issuercertificatethumbprint".
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment $deployment -Name "issuercertificatethumbprint" -ValueOnly</code>
    /// <para>This command return the value of parameter with name "issuercertificatethumbprint".
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>

    [Cmdlet(VerbsCommon.Get, "ISHDeploymentParameters")]
    public sealed class GetISHDeploymentParametersCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">Switch parameter to get data from original file</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Original flag will return initial parameters.", ParameterSetName = "Default")]
        [Parameter(Mandatory = false, HelpMessage = "Original flag will return initial parameters.", ParameterSetName = "SpecificParameter")]
        public SwitchParameter Original { get; set; }

        /// <summary>
        /// <para type="description">Switch parameter to get difference from changed and original file</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Changed flag will return changed parameters only.", ParameterSetName = "Default")]
        [Parameter(Mandatory = false, HelpMessage = "Changed flag will return changed parameters only.", ParameterSetName = "SpecificParameter")]
        public SwitchParameter Changed { get; set; }

        /// <summary>
        /// <para type="description">Switch parameter to show real passwords in parameters</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "ShowPassword flag will show real passwords.", ParameterSetName = "Default")]
        [Parameter(Mandatory = false, HelpMessage = "ShowPassword flag will show real passwords.", ParameterSetName = "SpecificParameter")]
        public SwitchParameter ShowPassword { get; set; }

        /// <summary>
        /// <para type="description">The name of the parameter to receive</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The name of the parameter to receive.", ParameterSetName = "SpecificParameter")]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Output only the value</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Output only the value.", ParameterSetName = "SpecificParameter")]
        public SwitchParameter ValueOnly { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (Changed && Original)
            {
                throw new Exception("Flags '-Original' and '-Changed' cannot be switched both.");
            }

            var operation = new GetISHDeploymentParametersOperation(Logger, ISHDeployment, Original, Changed, ShowPassword);

            var parametersContent = operation.Run().ToList();

            if (MyInvocation.BoundParameters.ContainsKey("Name"))
            {
                var parameter = parametersContent.SingleOrDefault(x => string.Equals(x.Name, Name, StringComparison.CurrentCultureIgnoreCase));

                if (parameter != null)
                {
                    if (MyInvocation.BoundParameters.ContainsKey("ValueOnly"))
                    {
                        ISHWriteOutput(parameter.Value);
                    }
                    else
                    {
                        ISHWriteOutput(parameter);
                    }
                }
                else
                {
                    throw new Exception($"Parameter with name '{Name}' was not found.");
                }
            }
            else
            {
                ISHWriteOutput(parametersContent);
            }
        }
    }
}
