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
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHExternalPreview
{
    /// <summary>
    /// Disables external preview for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHExternalPreviewOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHExternalPreviewOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public DisableISHExternalPreviewOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Disabling of InfoShare external preview");

            _invoker.AddAction(new SetAttributeValueAction(
                    logger,
                    InfoShareAuthorWebConfig.Path,
                    InfoShareAuthorWebConfig.ExternalPreviewModuleXPath,
                    InfoShareAuthorWebConfig.ExternalPreviewModuleAttributeName, 
                    "THE_FISHEXTERNALID_TO_USE"));

            _invoker.AddAction(new CommentNodeByXPathAction(
                    logger,
                    InfoShareAuthorWebConfig.Path,
                    new [] {
                        InfoShareAuthorWebConfig.SystemWebServerModulesAddTrisoftExternalPreviewModuleXPath,
                        InfoShareAuthorWebConfig.SectionExternalPreviewModuleXPath,
                        InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalPreviewModuleXPath
                    }));
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
