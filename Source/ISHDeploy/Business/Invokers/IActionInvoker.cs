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

using System.Collections.Generic;
using ISHDeploy.Common.Interfaces.Actions;

namespace ISHDeploy.Business.Invokers
{
    /// <summary>
    /// Executes the sequence of actions one by one
    /// </summary>
    public interface IActionInvoker
    {
        /// <summary>
		/// Adds Action into invocation list
        /// </summary>
		/// <param name="action">An action to invoke <see cref="T:ISHDeploy.Interfaces.Actions.IAction"/>.</param>
        void AddAction(IAction action);

        /// <summary>
        /// Adds range of actions into invocation list
        /// </summary>
        /// <param name="actions">An range of actions to invoke <see cref="T:ISHDeploy.Interfaces.Actions.IAction"/>.</param>
        void AddActionsRange(IEnumerable<IAction> actions);

        /// <summary>
        /// Invokes actions sequence execution
        /// </summary>
        void Invoke();

        /// <summary>
        /// Gets list of actions
        /// </summary>
        /// <returns>List of actions</returns>
        IEnumerable<IAction> GetActions();
    }
}
