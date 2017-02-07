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
﻿using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Actions.XmlFile;

namespace ISHDeploy.Business.Operations.ISHServiceTranslation
{
    /// <summary>
    /// Sets configuration of TMS.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIntegrationTmsOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceTranslationOrganizerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="tmsConfiguration">The TMS configuration.</param>
        public SetISHIntegrationTmsOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, BaseXMLElement tmsConfiguration) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting configuration of TMS");
            var filePath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, tmsConfiguration.RelativeFilePath);

            _invoker.AddAction(new SetElementAction(
                logger,
                filePath,
                tmsConfiguration));
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
