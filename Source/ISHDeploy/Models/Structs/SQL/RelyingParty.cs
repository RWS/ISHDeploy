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
﻿﻿using System.Data;
﻿using System.Security.Cryptography.X509Certificates;

namespace ISHDeploy.Models.Structs.SQL
{
    /// <summary>
    ///	<para type="description">Represents the installed Content Manager deployment.</para>
    /// </summary>
    public class RelyingParty
    {
        /// <summary>
        /// The relying party name
        /// </summary>
        public string Name;

        /// <summary>
        /// The relying party identifier
        /// </summary>
        public string Identifier;

        /// <summary>
        /// If relying party is enabled
        /// </summary>
        public bool  Enabled;

        /// <summary>
        /// The relying party encrypting certificate
        /// </summary>
        //private X509Certificate EncryptingCertificate;
        public string EncryptingCertificate;


        /// <summary>
        /// Initializes a new instance of the <see cref="ISHDeployment" /> class.
        /// </summary>
        /// <param name="name">The relying party name.</param>
        /// <param name="identifier">The relying party identifier.</param>
        /// <param name="enabled">if set to <c>true</c> then  relying party is enabled.</param>
        /// <param name="encryptingCertificate">The relying party encrypting certificate.</param>
        public RelyingParty(string name, string identifier, bool enabled, /*X509Certificate*/ string encryptingCertificate)
        {
            Name = name;
            Identifier = identifier;
            Enabled = enabled;
            EncryptingCertificate = encryptingCertificate;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DataTable"/> to <see cref="RelyingParty"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        static public implicit operator RelyingParty(DataRow value)
        {
            string id = (string)value["Identifier"];
            string name = (string)value["Name"];
            bool enabled = (bool)value["Enabled"];
            //X509Certificate cert = value["EncryptingCertificate"];
            string cert = (string)value["EncryptingCertificate"];

            return new RelyingParty(name, id, enabled, cert);
        }
    }
}
