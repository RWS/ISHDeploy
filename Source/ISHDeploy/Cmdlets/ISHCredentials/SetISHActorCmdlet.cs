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
using System.Runtime.InteropServices;
using ISHDeploy.Business.Operations.ISHCredentials;

namespace ISHDeploy.Cmdlets.ISHCredentials
{
    /// <summary>
    /// <para type="synopsis">Sets Actor user credentials.</para>
    /// <para type="description">The Set-ISHActor cmdlet sets new security credentials of Actor user.</para>
    /// <para type="link">Enable-ISHExternalPreview</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHActor -ISHDeployment $deployment -Credential $credential</code>
    /// <para>This command sets new Actor user credentials.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// Parameter $credential is a set of security credentials, such as a user name and a password.
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHActor")]
    public sealed class SetISHActorCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The credential of Actor user.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The credential of Actor user")]
        [ValidateNotNullOrEmpty]
        public PSCredential Credential { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new SetISHIssuerActorUserOperation(
                Logger,
                ISHDeployment,
                Credential.UserName,
                Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Credential.Password)));

            operation.Run();
        }
    }
}
