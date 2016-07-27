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
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Resets STS database
    /// </summary>
    /// <seealso cref="IOperation" />
    public class ResetISHSTSOperation : BasePathsOperation, IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public ResetISHSTSOperation(ILogger logger, Models.ISHDeploymentInternal ishDeployment) :
            base(logger, ishDeployment)
		{
			_invoker = new ActionInvoker(logger, "Reset STS database");

            _invoker.AddAction(new StopApplicationPoolAction(logger, ishDeployment.STSAppPoolName));
            _invoker.AddAction(new FileCleanDirectoryAction(logger, ishDeployment.WebNameSTSAppData));
            _invoker.AddAction(new RecycleApplicationPoolAction(logger, ishDeployment.STSAppPoolName, true));
            _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareSTSWebConfigPath));
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
