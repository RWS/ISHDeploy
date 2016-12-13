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
using ISHDeploy.Business.Operations.ISHPackage;

namespace ISHDeploy.Cmdlets.ISHPackage
{
    /// <summary>
    /// <para type="synopsis">Copies several files to Custom or Bin directory and replaces parameters in files on values of the deployment.</para>
    /// <para type="description">Will copy files from packages directory to Custom or Bin directory depends on switch and replace the placeholders of files with the corresponding values of the deployment. Replacement of values occurs only in files of certain extensions.</para>
    /// <para type="link">Set-ISHCMCUILResourceGroup</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\&gt;Copy-ISHCMFile -ISHDeployment $deployment -ToCustom -FileName test.js</code>
    /// <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Copy, "ISHCMFile")]
    [AdministratorRights]
    public class CopyISHCMFileCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Path to zip file.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Paths to files")]
        public string[] FileName { get; set; }

        /// <summary>
        /// <para type="description">Menu item move to the last position.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Copy not exist files to Custom folder", ParameterSetName = "ToCustom")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter ToCustom { get; set; }

        /// <summary>
        /// <para type="description">Menu item move to the first position.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Copy extra dll to Bin folder", ParameterSetName = "ToBin")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter ToBin { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new CopyISHCMFileOperation(Logger, ISHDeployment, FileName, ToBin.IsPresent);
            operation.Run();
        }
    }
}
