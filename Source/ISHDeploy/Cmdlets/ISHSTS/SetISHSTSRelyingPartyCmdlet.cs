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
    /// <para type="description">The Set-ISHSTSRelyingParty cmdlet sets relying party to from the infosharests database.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHSTSRelyingParty -ISHDeployment $deployment -Name "BL: WCF" -Realm "https://realm.example.com/BL/wcf/"</code>
    /// <code>PS C:\>Set-ISHSTSRelyingParty -ISHDeployment $deployment -Name "LC: WCF" -Realm "https://realm.example.com/LC/wcf/"</code>
    /// <code>PS C:\>Set-ISHSTSRelyingParty -ISHDeployment $deployment -Name "AL: Wcf/API/TestWithCertificate" -Realm "https://realm.example.com/wcf/Api/Test" -EncryptingCertificate "EncryptingCertificateSample="</code>
    /// <code>PS C:\>Set-ISHSTSRelyingParty -ISHDeployment $deployment -Name "QL: Wcf/API/Test" -Realm "https://realm.example.com/wcf/Api/Test"</code>
    /// <code>PS C:\>Set-ISHSTSRelyingParty -ISHDeployment $deployment -Name "Wcf/API/TestWithCertificate/" -Realm "https://realm.example.com/wcf/Api/Test" -EncryptingCertificate "EncryptingCertificateSample="</code>/// 
    /// <code>PS C:\>Set-ISHSTSRelyingParty -ISHDeployment $deployment -Name "Wcf/API/Test" -Realm "https://realm.example.com/wcf/Api/Test"</code>
    /// <para>This command sets relying parties to infosharests database.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// <para>As prefixes can be configured for custom handling, it is not possible to change it when updating existing data for relying party.</para>
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
        [Parameter(Mandatory = true, ParameterSetName = "LC", HelpMessage = "Relying parties for LiveContent")]
        public SwitchParameter LC { get; set; }

        /// <summary>
        /// <para type="description">Flag to set relying party for BlueLion.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "BL", HelpMessage = "Relying parties for BlueLion")]
        public SwitchParameter BL { get; set; }

        /// <summary>
        /// <para type="description">Relying party encryption certificate.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Active", HelpMessage = "Relying party encryption certificate.")]
        [ValidateNotNullOrEmpty]
        public string EncryptingCertificate { get; set; }

        private SetISHSTSRelyingPartyOperation.RelyingPartyType RelyingPartyType
        {
            get
            {
                if (LC)
                {
                    return SetISHSTSRelyingPartyOperation.RelyingPartyType.LC;
                }

                if (BL)
                {
                    return SetISHSTSRelyingPartyOperation.RelyingPartyType.BL;
                }

                return SetISHSTSRelyingPartyOperation.RelyingPartyType.None;
            }
        }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new SetISHSTSRelyingPartyOperation(Logger, ISHDeployment, Name, Realm, RelyingPartyType, EncryptingCertificate);

            operation.Run();
        }
    }
}
