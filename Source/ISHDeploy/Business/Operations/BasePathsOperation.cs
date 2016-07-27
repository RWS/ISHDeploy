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
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Interfaces;
﻿using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides xpaths, search patterns and constants of deployment files
    /// </summary>
    public abstract partial class BasePathsOperation
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger Logger;

        /// <summary>
        /// <para type="description">Internal extended description of the instance of the Content Manager deployment.</para>
        /// </summary>
        protected ISHDeploymentInternal Deployment { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePathsOperation"/> class.
        /// </summary>
        /// <param name="ishDeployment">The ish deployment.</param>
        /// <param name="logger"></param>
        protected BasePathsOperation(ILogger logger, Models.ISHDeployment ishDeployment)
        {
            Logger = logger;

            var action = new GetISHDeploymentExtendedAction(Logger, ishDeployment.Name,
                result => Deployment = result);
            action.Execute();
        }
    }
}
