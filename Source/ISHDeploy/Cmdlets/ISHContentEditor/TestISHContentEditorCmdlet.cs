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
﻿using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHContentEditor;

namespace ISHDeploy.Cmdlets.ISHContentEditor
{
    /// <summary>
    /// <para type="synopsis">Tests if Content Editor license exists for specific domain name.</para>
    /// <para type="description">Test-ISHContentEditor cmdlet tests if Content Editor license exists for specific domain name.</para>
    /// <para type="description">If license for 'com' domain was created then all domains that ends with '.com' will be valid.</para>
    /// <para type="description">In that case localhost.com domain will be valid, but localhost.com.net will be invalid.</para>
    /// <para type="link">Set-ISHContentEditor</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Test-ISHContentEditor -Domain "localhost" -ISHDeployment $deployment</code>
    /// <para>This command checks if license for domain name 'localhost' exists.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsDiagnostic.Test, "ISHContentEditor")]
	public class TestISHContentEditorCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the domain name to be verified.</para>
        /// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Domain name to be verified")]
		[ValidateNotNullOrEmpty]
		public string Domain { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
		public override void ExecuteCmdlet()
		{
            var operation = new TestISHContentEditorOperation(Logger, ISHDeployment, Domain);

            var result = operation.Run();

            ISHWriteOutput(result);
		}
	}
}