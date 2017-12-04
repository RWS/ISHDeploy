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

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Base cmdlet class to work with credentials
    /// </summary>
    public abstract class BasePSCredentialCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">The credential that will be set.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The credential that will be set")]
        [ValidateNotNullOrEmpty]
        public virtual PSCredential Credential { private get; set; }

        /// <summary>
        /// <para type="description">The user name.</para>
        /// </summary>
        protected string CredentialUserName { get; }

        /// <summary>
        /// <para type="description">The password.</para>
        /// </summary>
        protected string CredentialPassword { get; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected BasePSCredentialCmdlet()
        {
            CredentialUserName = Credential.UserName;
            CredentialPassword = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Credential.Password));
        }
    }
}
