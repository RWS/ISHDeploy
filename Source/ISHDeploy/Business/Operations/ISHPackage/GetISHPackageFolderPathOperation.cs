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
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// Gets the path to the packages folder
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class GetISHPackageFolderPathOperation : BasePathsOperation, IOperation<string>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Return path in UNC format
        /// </summary>
        private bool _isUNCFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHPackageFolderPathOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public GetISHPackageFolderPathOperation(ILogger logger, Models.ISHDeployment ishDeployment, bool isUNCFormat = false) :
            base(logger, ishDeployment)
        {
            _isUNCFormat = isUNCFormat;

            _invoker = new ActionInvoker(logger, "Getting the path to the packages folder");

            _invoker.AddAction(new DirectoryEnsureExistsAction(logger, Deployment.PackagesFolderPath));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public string Run()
        {
            _invoker.Invoke();

            return _isUNCFormat ? Deployment.PackagesFolderUNCPath : Deployment.PackagesFolderPath; ;
        }
    }
}
