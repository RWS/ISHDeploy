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

using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Business.Operations.ISHComponent;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Starts deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class StopISHDeploymentOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHComponentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public StopISHDeploymentOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            ishDeployment.Status = ISHDeploymentStatus.Stopped;

            Models.ISHComponent[] components;
            if (fileManager.FileExists(CurrentISHComponentStatesFilePath.AbsolutePath))
            {
                var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
                var componentsCollection = dataAggregateHelper.ReadComponentsFromFile(CurrentISHComponentStatesFilePath.AbsolutePath);

                components = componentsCollection.Components.Where(x => x.IsEnabled).ToArray();
            }
            else
            {
                components = new Models.ISHComponentsCollection(true).Components.ToArray();
            }

            IOperation operation = new DisableISHComponentOperation(logger, ishDeployment, false, components);
            Invoker = new ActionInvoker(Logger, "Stopping of enabled components", operation.Invoker.GetActions());

            Invoker.AddAction(new SaveISHDeploymentStatusAction(ishDeployment.Name, ISHDeploymentStatus.Stopped));
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
