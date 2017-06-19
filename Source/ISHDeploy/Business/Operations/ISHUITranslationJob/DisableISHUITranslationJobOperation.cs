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
using ISHDeploy.Data.Actions.TextFile;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Common.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHUITranslationJob
{
    /// <summary>
    /// Disables translation job functionality for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHUITranslationJobOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHUITranslationJobOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public DisableISHUITranslationJobOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Disabling of InfoShare translation job");
            
            Invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, EventMonitorMenuBarXmlPath, EventMonitorMenuBarXml.EventMonitorTranslationJobs));
            Invoker.AddAction(new CommentNodeByXPathAction(logger, TopDocumentButtonBarXmlPath, TopDocumentButtonBarXml.TopDocumentTranslationJobXPath, true));
            Invoker.AddAction(new CommentBlockAction(logger, AuthorASPTreeHtmPath, AuthorASPTreeHtm.TranslationJobHack));
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
