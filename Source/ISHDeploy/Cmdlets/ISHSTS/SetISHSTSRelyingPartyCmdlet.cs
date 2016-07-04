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

using System.Management.Automation;
﻿using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHSTS
{
    /// <summary>
    /// <para type="synopsis">Sets relying party to infosharests database. The switches acts as filter based on the RP name.</para>
    /// <para type="description">The Get-ISHSTSRelyingParty cmdlet gets all relying parties from the infosharests database.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -ISH</code>
    /// <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -LC</code>
    /// <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -BL</code>
    /// <code>PS C:\>Get-ISHSTSRelyingParty -ISHDeployment $deployment -ISH -BL -LC #Any combination</code>
    /// <para>This command gets t all relying parties from the infosharests database.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHSTSRelyingParty")]
    public class SetISHSTSRelyingPartyCmdlet : BaseISHDeploymentCmdlet
    {
        /// <summary>
        /// <para type="description">Relying party name.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Relying party name")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Relying party realm.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Relying party realm")]
        [ValidateNotNullOrEmpty]
        public string Realm { get; set; }
        
        /// <summary>
        /// <para type="description">Flag to set relying party for LiveContent.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Relying parties for LiveContent")]
        public SwitchParameter LC { get; set; }

        /// <summary>
        /// <para type="description">Flag to set relying party for BlueLion.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Relying parties for BlueLion")]
        public SwitchParameter BL { get; set; }

        /// <summary>
        /// <para type="description">Relying party encryption certificate.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Relying party encryption certificate.")]
        [ValidateNotNullOrEmpty]
        public string EncryptionCertificate { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            //var operation = new SetISHSTSRelyingPartyOperation(Logger, ISHDeployment, LC, BL);
            var operation = new SetISHSTSRelyingPartyOperation(Logger, ISHDeployment, Name, Realm, "", EncryptionCertificate);

            operation.Run();
        }
    }
}
