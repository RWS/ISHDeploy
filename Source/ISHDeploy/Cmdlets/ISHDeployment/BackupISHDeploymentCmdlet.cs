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
    /// <para type="synopsis">Backup data from Web, App, Data folders.</para>
    /// <para type="description">Will Backup data from Web, App, Data folders depends on switch.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Backup-ISHDeployment -Web -Path Author\ASP\Web.config </code>
    /// <code>PS C:\>Backup-ISHDeployment -Web -Path bin\*.txt </code>
    /// <code>PS C:\>Backup-ISHDeployment -Web -Path Trisoft*.dll </code>
    /// <para></para>
    /// </example>
    [Cmdlet(VerbsData.Backup, "ISHDeployment")]
    public class BackupISHDeploymentCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">Path to files for backup.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Paths to files")]
        public string[] Path { get; set; }

        /// <summary>
        /// <para type="description">Working with Web folder.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Backup files from Web folder", ParameterSetName = "Web")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Web { get; set; }

        /// <summary>
        /// <para type="description">Working with App folder.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Backup files from App folder", ParameterSetName = "App")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter App { get; set; }

        /// <summary>
        /// <para type="description">Working with Data folder.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Backup files from Data folder", ParameterSetName = "Data")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Data { get; set; }       
        
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new BackupISHDeploymentOperation(Logger, ISHDeployment, ParameterSetName, Path);
            operation.Run();
        }
    }
}
