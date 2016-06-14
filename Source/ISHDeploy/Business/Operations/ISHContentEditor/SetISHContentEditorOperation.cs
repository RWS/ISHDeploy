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
﻿using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHContentEditor
{
    /// <summary>
    /// Sets new license for Content Editor
    /// </summary>
    public class SetISHContentEditorOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="fileName">Name of the file that will be created.</param>
        /// <param name="fileContent">Content of the new file.</param>
        public SetISHContentEditorOperation(ILogger logger, Models.ISHDeployment ishDeployment, string fileName, string fileContent) : 
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting of new license for Content Editor");

            _invoker.AddAction(new FileCreateAction(logger, FoldersPaths.LicenceFolderPath, fileName, fileContent));
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