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

using System;
using System.Collections.Generic;
using ISHDeploy.Business.Invokers;
﻿using ISHDeploy.Data.Actions.DataBase;
﻿using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Sets the Relying Party
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class SetISHSTSRelyingPartyOperation : BasePathsOperation
    {
        /// <summary>
        /// Operation type enum
        ///	<para type="description">Enumeration of Relying parties Types.</para>
        /// </summary>
        public enum RelyingPartyType
        {
            /// <summary>
            /// Flag to identify Info Share
            /// </summary>
            ISH,

            /// <summary>
            /// Flag to identify Live Content
            /// </summary>
            LC,

            /// <summary>
            /// Flag to identify Blue Lion
            /// </summary>
            BL
        }

        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHSTSRelyingPartyOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="name">The name.</param>
        /// <param name="realm">The realm.</param>
        /// <param name="relyingPartyType">The relying party type.</param>
        /// <param name="encryptionCertificate">The encryption certificate.</param>
        public SetISHSTSRelyingPartyOperation(ILogger logger, Models.ISHDeployment ishDeployment, string name, string realm, RelyingPartyType relyingPartyType, string encryptionCertificate):
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Getting the path to the packages folder");
            _invoker.AddAction(new SqlCompactInsertUpdateAction(logger,
                        //InfoShareSTSDataBase.ConnectionString,
                        $"Data Source = C:\\ISHSandbox\\WebSQL2014\\InfoShareSTS\\App_Data\\IdentityServerConfiguration-2.2.sdf",
                        InfoShareSTSDataBase.RelyingPartyTableName,
                        "Realm",
                        new Dictionary <string, object>
                        {
                            { "Name", $"{relyingPartyType}: {name}"},
                            { "Realm", realm},
                            { "EncryptingCertificate", encryptionCertificate},
                            { "Enabled", 1},
                            { "TokenLifeTime", 0}
                        }));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
