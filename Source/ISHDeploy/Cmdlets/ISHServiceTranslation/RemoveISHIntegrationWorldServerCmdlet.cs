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

using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using ISHDeploy.Business.Operations.ISHServiceTranslation;
using ISHDeploy.Common.Models.TranslationOrganizer;

namespace ISHDeploy.Cmdlets.ISHServiceTranslation
{
    /// <summary>
	///		<para type="synopsis">World Server Instance from Translation Organizer.</para>
	///		<para type="description">The Remove-ISHIntegrationWorldServer cmdlet removes World Server Instance from Translation Organizer.</para>
	///		<para type="link">Set-ISHIntegrationWorldServer</para>
	/// </summary>
	/// <example>
	///		<code>PS C:\>Remove-ISHIntegrationWorldServer -ISHDeployment $deployment</code>
	///		<para>Removes World Server Instance from Translation Organizer.
	/// This command removes World Server Instance from Translation Organizer.
	/// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
    [Cmdlet(VerbsCommon.Remove, "ISHIntegrationWorldServer")]
    public sealed class RemoveISHIntegrationWorldServerCmdlet : BaseHistoryEntryCmdlet
    {
              
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new RemoveISHIntegrationWorldServerOperation (Logger, ISHDeployment);
            operation.Run();
        }
    }
}
