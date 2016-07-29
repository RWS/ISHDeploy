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

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Acquire the input parameters of a specific deployment.</para>
    /// <para type="description">The Get-ISHDeploymentParameters cmdlet gets parameters for Content Manager deployment.</para>
    /// <para type="link">Get-ISHDeployment</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment $deployment</code>
    /// <para>This command gets the input parameters of Content Manager deployment with hidden passwords.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment "InfoShare" -ShowPassword</code>
    /// <para>This command gets the input parameters of "InfoShare" deployment with real passwords.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment "InfoShare" -Original</code>
    /// <para>This command return original values (from the backup folder) of "InfoShare" deployment.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentParameters -ISHDeployment "InfoShare" -Changed</code>
    /// <para>This command return only changed parameters of "InfoShare" deployment.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>

    [Cmdlet(VerbsCommon.Get, "ISHDeploymentParameters")]
    public sealed class GetISHDeploymentParametersCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Switch parameter to get data from original file
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Original flag will return initial parameters.")]
        public SwitchParameter Original
        {
            get { return _Original; }
            set { _Original = value; }
        }
        private bool _Original;
        
        /// <summary>
        /// Switch parameter to get difference from changed and original file
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "Changed flag will return changed parameters only.")]
        public SwitchParameter Changed
        {
            get { return _Changed; }
            set { _Changed = value; }
        }
        private bool _Changed;
        
        /// <summary>
        /// Switch parameter to show real passwords in parameters
        /// </summary>
        [Parameter(Position = 0, HelpMessage = "ShowPassword flag will show real passwords.")]
        public SwitchParameter ShowPassword
        {
            get { return _ShowPassword; }
            set { _ShowPassword = value; }
        }

        private bool _ShowPassword;
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

            var parametersContent = operation.Run();

            foreach (var pc in parametersContent)
            {
                WriteObject(pc);
            }
        }
    }
}
