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
using ISHDeploy.Business.Operations.ISHIntegrationSTSWS;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTS
{
    /// <summary>
    ///		<para type="synopsis">Saves STS integration configuration to zip archive in package folder.</para>
    ///		<para type="description">The Save-ISHIntegrationSTSConfigurationPackage saves STS integration configuration to zip archive in package folder.</para>
    ///     <para type="description">Cmdlet will overwrite zip archive if there is already one with same name.</para>
    ///     <para type="link">Disable-ISHIntegrationSTSInternalAuthenticationCmdlet</para>
    ///     <para type="link">Enable-ISHIntegrationSTSInternalAuthenticationCmdlet</para>
    ///     <para type="link">Remove-ISHIntegrationSTSCertificateCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSCertificateCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSFederationCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSTrustCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSFederationCmdlet</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.ISHIntegrationSTS" />
    /// <example>
    ///		<code>PS C:\>Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment -FileName packageFile.zip</code>
    ///     <para>Creates a zip archive in package folder with current STS integration configurations.
    ///         Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    /// <example>
    ///		<code>PS C:\>Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment -ADFS -FileName packageFile.zip</code>
    ///     <para>Creates a zip archive in package folder with current STS integration configurations for ADFS.
    ///         Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    [Cmdlet(VerbsData.Save, "ISHIntegrationSTSConfigurationPackage")]
    public class SaveISHIntegrationSTSConfigurationPackageCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Name of the output package file.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Name of the output package file.")]
        public string FileName { get; set; }

        /// <summary>
        /// <para type="description">Specifies that ADFS configuration should be packed.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Used to export ADFS configuration.")]
        public SwitchParameter ADFS { get; set; }

        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new SaveISHIntegrationSTSConfigurationPackageOperation(Logger, ISHDeployment, FileName, ADFS.IsPresent);

            operation.Run();
        }
    }
}
