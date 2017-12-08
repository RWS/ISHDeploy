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

using ISHDeploy.Business.Invokers;
using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models.Backup;
using ISHDeploy.Data.Actions.Registry;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHComponent
{
    /// <summary>
    /// Sets ISH builders registry value.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHBuildersRegistryValueOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHBuildersRegistryValueOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="registryValueName">The name of registry value.</param>
        /// <param name="value">The value.</param>
        public SetISHBuildersRegistryValueOperation(ILogger logger, Models.ISHDeployment ishDeployment, RegistryValueName registryValueName, object value) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, $"Setting of ISH builders registry value");

            Invoker.AddAction(new SetRegistryValueAction(logger, new RegistryValue { Key = RegInfoShareBuildersRegistryElement, ValueName = registryValueName, Value = value }, VanillaRegistryValuesFilePath));
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
