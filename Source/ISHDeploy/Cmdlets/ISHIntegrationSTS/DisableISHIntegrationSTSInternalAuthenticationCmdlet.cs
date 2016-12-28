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
using ISHDeploy.Business.Operations.ISHIntegrationSTS;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTS
{
    /// <summary>
    ///		<para type="synopsis">Removes additional login redirect.</para>
    ///		<para type="description">Deleted directory with created for internal login STS files.</para>
    ///     <para type="link">Enable-ISHIntegrationSTSInternalAuthentication</para>
    ///     <para type="link">Remove-ISHIntegrationSTSCertificate</para>
    ///     <para type="link">Set-ISHIntegrationSTSCertificate</para>
    ///     <para type="link">Save-ISHIntegrationSTSConfigurationPackage</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSFederation</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSTrust</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.ISHIntegrationSTS" />
    /// <example>
    ///		<code>PS C:\&gt;Disable-ISHIntegrationSTSInternalAuthentication -ISHDeployment $deployment</code>
    ///		<para>This command disable internal STS authentication and removes files created for internal STS login.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>

    [Cmdlet(VerbsLifecycle.Disable, "ISHIntegrationSTSInternalAuthentication")]
    public sealed class DisableISHIntegrationSTSInternalAuthenticationCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHIntegrationSTSInternalAuthenticationOperation(Logger, ISHDeployment);

            operation.Run();
        }
    }
}