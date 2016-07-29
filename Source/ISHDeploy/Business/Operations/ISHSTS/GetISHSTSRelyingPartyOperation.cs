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
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Business.Invokers;
﻿using ISHDeploy.Data.Actions.DataBase;
﻿using ISHDeploy.Interfaces;
using ISHDeploy.Models.SQL;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Gets the configured Relying Parties
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    /// <seealso cref="IOperation" />
    public class GetISHSTSRelyingPartyOperation : BaseOperationPaths, IOperation<IEnumerable<RelyingParty>>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The list of configured Relying Parties found.
        /// </summary>
        private IEnumerable<RelyingParty> _resultRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHSTSRelyingPartyOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="ISH">If set to <c>true</c> then add parties to ISH.</param>
        /// <param name="LC">If set to <c>true</c> then add parties to Live Content.</param>
        /// <param name="BL">If set to <c>true</c> then add parties to Blue Lion.</param>
        public GetISHSTSRelyingPartyOperation(ILogger logger, Models.ISHDeployment ishDeployment, bool ISH, bool LC, bool BL):
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Getting the relying parties");

            string sqlQuery = InfoShareSTSDataBase.GetRelyingPartySQLCommandFormat;

            List<string> conditions = new List<string>();

            if (ISH)
            {
                conditions.Add("ISH");
            }

            if (LC)
            {
                conditions.Add("LC");
            }

            if (BL)
            {
                conditions.Add("BL");
            }

            if (conditions.Count > 0)
            {
                sqlQuery += " WHERE " + String.Join(" OR ", conditions.Select(x => $"Name LIKE '{x}%'"));
            }

            _invoker.AddAction(new SqlCompactSelectAction<RelyingParty>(logger,
                    InfoShareSTSDataBaseConnectionString,
                    sqlQuery,
                    result =>
                    {
                        _resultRows = result;
                    }));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public IEnumerable<RelyingParty> Run()
        {
            _invoker.Invoke();

            return _resultRows;
        }
    }
}
