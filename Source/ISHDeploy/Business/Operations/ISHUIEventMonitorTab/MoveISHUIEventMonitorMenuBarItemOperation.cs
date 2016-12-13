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
	/// Moves Event Monitor Tab".
	/// </summary>
	/// <seealso cref="IOperation" />
	public class MoveISHUIEventMonitorMenuBarItemOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// Operation type enum
        ///	<para type="description">Enumeration of Operations Types.</para>
        /// </summary>
        public enum OperationType
		{

			/// <summary>
			/// Flag to insert after 
			/// </summary>
			InsertAfter,

			/// <summary>
			/// Flag to insert before
			/// </summary>
			InsertBefore
		}

        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveISHUIEventMonitorMenuBarItemOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="label">Label of the element</param>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="targetLabel">The target label.</param>
        public MoveISHUIEventMonitorMenuBarItemOperation(ILogger logger, Models.ISHDeployment ishDeployment, string label, OperationType operationType, string targetLabel = null) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Moving of Event Monitor Tab");

			string nodeXPath = string.Format(EventMonitorMenuBarXml.EventMonitorTab, label);
			string nodeCommentXPath = nodeXPath + EventMonitorMenuBarXml.EventMonitorPreccedingCommentXPath;

			string targetNodeXPath = string.IsNullOrEmpty(targetLabel) ? null : string.Format(EventMonitorMenuBarXml.EventMonitorTab, targetLabel);

			// Combile node and its xPath
			string nodesToMoveXPath = nodeXPath + "|" + nodeCommentXPath;

			switch (operationType)
	        {
				case OperationType.InsertAfter:
					_invoker.AddAction(new MoveAfterNodeAction(logger, EventMonitorMenuBarXmlPath, nodesToMoveXPath, targetNodeXPath));
					break;
				case OperationType.InsertBefore:
					_invoker.AddAction(new MoveBeforeNodeAction(logger, EventMonitorMenuBarXmlPath, nodesToMoveXPath, targetNodeXPath));
					break;
			}
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
