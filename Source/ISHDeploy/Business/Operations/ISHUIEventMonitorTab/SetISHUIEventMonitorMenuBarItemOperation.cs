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
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models.ISHXmlNodes;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHUIEventMonitorTab
{
    /// <summary>
    /// Sets Event Monitor Tab.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHUIEventMonitorMenuBarItemOperation : BaseOperationPaths, IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHUIEventMonitorMenuBarItemOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="menuItem">The menu item object.</param>
        public SetISHUIEventMonitorMenuBarItemOperation(ILogger logger, Models.ISHDeployment ishDeployment, EventLogMenuItem menuItem) :
            base(logger, ishDeployment)
		{
			Invoker = new ActionInvoker(logger, "Setting of Event Monitor Tab");

			Invoker.AddAction(new SetNodeAction(logger, EventMonitorMenuBarXmlPath, string.Format(EventMonitorMenuBarXml.EventMonitorTab, menuItem.Label), menuItem));
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
