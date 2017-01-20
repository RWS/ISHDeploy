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
        /// The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".
        /// </summary>
        private readonly string _keyName;

        /// <summary>
        /// The name of the name/value pair.
        /// </summary>
        private readonly string _valueName;

        /// <summary>
        /// The value to be stored.
        /// </summary>
        private readonly object _value;

        /// <summary>
        /// The registry manager
        /// </summary>
        private readonly IRegistryManager _registryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetRegistryValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="keyName">The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".</param>
        /// <param name="valueName">The name of the name/value pair.</param>
        /// <param name="value">The value to be stored.</param>
        public SetRegistryValueAction(ILogger logger, string keyName, string valueName, object value) 
			: base(logger)
        {
            _keyName = keyName;
            _valueName = valueName;
            _value = value;

            _registryManager = ObjectFactory.GetInstance<IRegistryManager>();
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
            _registryManager.SetRegistryValue(_keyName, _valueName, _value);
		}
	}
}
