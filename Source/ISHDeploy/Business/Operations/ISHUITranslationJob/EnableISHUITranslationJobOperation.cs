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
using ISHDeploy.Data.Actions.TextFile;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUITranslationJob
{
    /// <summary>
    /// Enables translation job functionality for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHUITranslationJobOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHUITranslationJobOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public EnableISHUITranslationJobOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Enabling of InfoShare translation job");
            
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, EventMonitorMenuBarXml.Path, EventMonitorMenuBarXml.EventMonitorTranslationJobs));
            _invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, TopDocumentButtonBarXml.Path, TopDocumentButtonBarXml.TranslationJobAttribute, true));
            _invoker.AddAction(new UncommentBlockAction(logger, AuthorASPTreeHtm.Path, AuthorASPTreeHtm.TranslationJobHack));
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
