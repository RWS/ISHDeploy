/**
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
using ISHDeploy.Business.Operations.ISHPackage;

namespace ISHDeploy.Cmdlets.ISHPackage
{
    /// <summary>
    /// <para type="synopsis">Gets the path of a folder where the commandlets output files or packages or use them as input.</para>
    /// <para type="description">The Get-ISHPackageFolderPath cmdlet gets the path of a folder where the commandlets output files or packages or use them as input. This folder is specific per deployment.
    /// The folder contains archives or files that were created as output of other cmdlets of the module.
    /// The folder contains also archives or files that are used as input to other cmdlets of the module.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHPackageFolderPath -ISHDeployment $deployment -UNC</code>
    /// <para>This command gets the UNC path to Packages folder for Content Manager deployment.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHPackageFolderPath")]
    public class GetISHPackageFolderPathCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">Return path in UNC format.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Result path format")]
        public SwitchParameter UNC { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new GetISHPackageFolderPathOperation(Logger, ISHDeployment, UNC);

            var result = operation.Run();

            WriteObject(result);
        }
    }
}
