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
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.License;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHContentEditor
{
    /// <summary>
    /// Tests if license for specific host name exists
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class TestISHContentEditorOperation : BaseOperationPaths, IOperation<bool>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// The operation result.
        /// </summary>
        private bool _isLicenceValid = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestISHContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="domain">The host name is checked for the existence of the license file.</param>
        public TestISHContentEditorOperation(ILogger logger, Models.ISHDeployment ishDeployment, string domain) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Testing of license for specific host name");
            Invoker.AddAction(new LicenseTestAction(logger, LicenceFolderPath, domain, isValid => { _isLicenceValid = isValid; }));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public bool Run()
        {
            Invoker.Invoke();

            return _isLicenceValid;
        }
    }
}