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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.DataBase;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models.SQL;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Sets the Relying Party
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    /// <seealso cref="IOperation" />
    public class SetISHSTSRelyingPartyOperation : BaseOperationPaths
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHSTSRelyingPartyOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="name">The name.</param>
        /// <param name="realm">The realm.</param>
        /// <param name="relyingPartyType">The relying party type.</param>
        /// <param name="encryptingCertificate">The encryption certificate.</param>
        public SetISHSTSRelyingPartyOperation(ILogger logger, Models.ISHDeployment ishDeployment, string name, string realm, RelyingPartyType relyingPartyType, string encryptingCertificate) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Setting the relying parties");

            // Ensure DataBase file exists
            bool isDataBaseFileExist = false;
            (new FileExistsAction(logger, InfoShareSTSDataBasePath.AbsolutePath, returnResult => isDataBaseFileExist = returnResult)).Execute();
            if (!isDataBaseFileExist)
            {
                Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName, true));
                Invoker.AddAction(new SqlCompactEnsureDataBaseExistsAction(logger, InfoShareSTSDataBasePath.AbsolutePath, $"{InputParameters.BaseUrl}/{InputParameters.STSWebAppName}"));
                Invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareSTSDataBasePath));
            }

            string relyingPartyTypePrefix;
            relyingPartyTypePrefix = relyingPartyType == RelyingPartyType.None ? Regex.Match(name, @"^(?<prefix>[A-Z]+)\:").Groups["prefix"].Value : relyingPartyType.ToString();

            if (!string.IsNullOrEmpty(relyingPartyTypePrefix))
            {
                int resultRowsCount = 0;
                Invoker.AddAction(new SqlCompactSelectAction<RelyingParty>(logger,
                    InfoShareSTSDataBaseConnectionString,
                    $"{InfoShareSTSDataBase.GetRelyingPartySQLCommandFormat} WHERE Realm ='{realm}' AND Name NOT LIKE '{relyingPartyTypePrefix}:%'",
                    result =>
                    {
                        resultRowsCount = result.Count();
                    }));

                Invoker.AddAction(new AssertAction(logger,
                    () => (resultRowsCount != 0),
                    $"Relying party prerfix can not be changed to '{relyingPartyTypePrefix}'."));
            }

            Invoker.AddAction(new SqlCompactInsertUpdateAction(logger,
                        InfoShareSTSDataBaseConnectionString,
                        InfoShareSTSDataBase.RelyingPartiesTableName,
                        "Realm",
                        new Dictionary<string, object>
                        {
                            { "Name", (relyingPartyType == RelyingPartyType.None) ? name : $"{relyingPartyType}: {name}"},
                            { "Realm", realm},
                            { "EncryptingCertificate", encryptingCertificate},
                            { "Enabled", 1},
                            { "TokenLifeTime", 0}
                        }));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }
    }
}
