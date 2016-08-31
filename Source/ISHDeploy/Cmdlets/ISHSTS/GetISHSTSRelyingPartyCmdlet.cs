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
using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHSTS
{
    /// <summary>
    ///     <para type="synopsis">Gets all relying parties from the infosharests database. The switches acts as filter based on the RP name.</para>
    ///     <para type="description">The Get-ISHSTSRelyingParty cmdlet gets all relying parties from the infosharests database.</para>
    ///     <para type="link">Remove-ISHIntegrationSTSCertificateCmdlet</para>
    ///     <para type="link">Reset-ISHSTSCmdlet</para>
    ///     <para type="link">Set-ISHIntegrationSTSCertificateCmdlet</para>
    ///     <para type="link">Set-ISHSTSConfigurationCmdlet</para>
    ///     <para type="link">Set-ISHSTSRelyingPartyCmdlet</para>
    /// </summary>
    /// <example>
    ///     <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -ISH</code>
    ///     <para>This command gets all relying parties from the infosharests database where name of relying party starts with "ISH".</para>
    /// </example>
    /// <example>
    ///     <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -LC</code>
    ///     <para>This command gets all relying parties from the infosharests database where name of relying party starts with "LC".</para>
    /// </example>
    /// <example>
    ///     <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -BL</code>
    ///     <para>This command gets all relying parties from the infosharests database where name of relying party starts with "BL".</para>
    /// </example>
    /// <example>
    ///     <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -ISH -BL -LC #Any combination</code>
    ///     <para>This command gets all relying parties from the infosharests database.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHSTSRelyingParty")]
    public class GetISHSTSRelyingPartyCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">Flag to return relying parties for InfoShare.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Relying parties for InfoShare")]
        public SwitchParameter ISH { get; set; }

        /// <summary>
        /// <para type="description">Flag to return relying parties for LiveContent.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Relying parties for LiveContent")]
        public SwitchParameter LC { get; set; }

        /// <summary>
        /// <para type="description">Flag to return relying parties for BlueLion.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Relying parties for BlueLion")]
        public SwitchParameter BL { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new GetISHSTSRelyingPartyOperation(Logger, ISHDeployment, ISH, LC, BL);

            var result = operation.Run();

            ISHWriteOutput(result);
        }
    }
}
