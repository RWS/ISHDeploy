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
﻿using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Adds entry to the history file about cmdlets usage
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    /// <seealso cref="IOperation" />
    public class AddHistoryEntryOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHistoryEntryOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="text">The text with description which cmdlet was executed with which parameters.</param>
        public AddHistoryEntryOperation(ILogger logger, Models.ISHDeployment ishDeployment, string text) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Adding of entry to the history file about cmdlets usage");
            _invoker.AddAction(new FileAddHistoryEntryAction(logger, HistoryFilePath, text, ishDeployment.Name));
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
