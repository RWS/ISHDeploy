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
        public virtual PSCredential Credential { get; set; }

        /// <summary>
        /// <para type="description">The user name.</para>
        /// </summary>
        protected string CredentialUserName { get; private set; }

        /// <summary>
        /// <para type="description">The password.</para>
        /// </summary>
        protected string CredentialPassword { get; private set; }

        /// <summary>
        /// Overrides BeginProcessing from base Cmdlet class with additinal debug information
        /// </summary>
        protected override void BeginProcessing()
        {
            CredentialUserName = NormalizeUserName(Credential.UserName);
            CredentialPassword = Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(Credential.Password));
            base.BeginProcessing();
        }

        /// <summary>
        /// UserName normalization
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string NormalizeUserName(string userName)
        {
            if (userName.StartsWith(@".\"))
            {
                Logger.WriteWarning($@"Credentials normalization.Replaced .\ with {Environment.MachineName}");
                return $@"{Environment.MachineName}{userName.Substring(2)}";
            }
            else if (userName.IndexOf(@"\", StringComparison.Ordinal) < 0)
            {
                Logger.WriteWarning($"Credentials normalization.Prefixed with {Environment.MachineName}");
                return $@"{Environment.MachineName}\{userName}";
            }

            return userName;
        }
    }
}
