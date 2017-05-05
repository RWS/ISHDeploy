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

using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Actions.Registry
{
    /// <summary>
    /// Implements registry set value action
    /// </summary>
    public class SetRegistryValueAction : BaseAction
    {
        /// <summary>
        /// The name of the name/value pair.
        /// </summary>
        private readonly RegistryValueName _registryValueName;

        /// <summary>
        /// The value to be stored.
        /// </summary>
        private readonly object _value;

        /// <summary>
        /// Additional parameters.
        /// </summary>
        private readonly object[] _parameters;

        /// <summary>
        /// The registry manager
        /// </summary>
        private readonly ITrisoftRegistryManager _registryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetRegistryValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="registryValueName">The name of the name/value pair</param>
        /// <param name="value">The value to be stored.</param>
        /// <param name="parameters">Additional parameters</param>
        public SetRegistryValueAction(ILogger logger, RegistryValueName registryValueName, object value, params object[] parameters) 
			: base(logger)
        {
            _registryValueName = registryValueName;
            _value = value;
            _parameters = parameters;

            _registryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
            _registryManager.SetRegistryValue(_registryValueName, _value, _parameters);
		}
	}
}
