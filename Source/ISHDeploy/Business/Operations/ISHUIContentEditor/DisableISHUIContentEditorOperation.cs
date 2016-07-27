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
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIContentEditor
{
    /// <summary>
    /// Disables Content Editor for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHUIContentEditorOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHUIContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public DisableISHUIContentEditorOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Disabling of InfoShare Content Editor");
            
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, Deployment.FolderButtonBarXmlPath, new [] { FolderButtonBarXml.XopusAddCheckOut, FolderButtonBarXml.XopusAddUndoCheckOut }));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, Deployment.InboxButtonBarXmlPath, InboxButtonBarXml.XopusAddCheckOut));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, Deployment.InboxButtonBarXmlPath, new [] { InboxButtonBarXml.XopusRemoveCheckoutDownload, InboxButtonBarXml.XopusRemoveCheckIn }));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, Deployment.LanguageDocumentButtonbarXmlPath, LanguageDocumentButtonbarXml.XopusAddCheckOut));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, Deployment.LanguageDocumentButtonbarXmlPath, new[] { LanguageDocumentButtonbarXml.XopusRemoveCheckoutDownload, LanguageDocumentButtonbarXml.XopusRemoveCheckIn }));
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
