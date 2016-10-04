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

namespace ISHDeploy.Business.Operations.ISHUIEventMonitorMenuBarItem
{
	/// <summary>
	/// Removes Event Monitor Tab".
	/// </summary>
	/// <seealso cref="IOperation" />
	public class RemoveISHUIEventMonitorMenuBarItemOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveISHUIEventMonitorMenuBarItemOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="label">Label of the element</param>
        public RemoveISHUIEventMonitorMenuBarItemOperation(ILogger logger, Models.ISHDeployment ishDeployment, string label) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Remove of Event Monitor Tab");

			string itemXPath = string.Format(EventMonitorMenuBarXml.EventMonitorTab, label);
			string itemCommentXPath = itemXPath + EventMonitorMenuBarXml.EventMonitorPreccedingCommentXPath;

			// First we should remove comment as it is dependent to its sibling node
			_invoker.AddAction(new RemoveSingleNodeAction(logger, EventMonitorMenuBarXmlPath, itemCommentXPath));

			// Then we removing item itself
			_invoker.AddAction(new RemoveSingleNodeAction(logger, EventMonitorMenuBarXmlPath, itemXPath));
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
