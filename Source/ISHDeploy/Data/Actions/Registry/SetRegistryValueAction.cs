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
using ISHDeploy.Common.Models.Backup;

namespace ISHDeploy.Data.Actions.Registry
{
    /// <summary>
    /// Implements registry set value action
    /// </summary>
    public class SetRegistryValueAction : BaseAction
    {
        /// <summary>
        /// The value to be stored.
        /// </summary>
        private readonly RegistryValue _registryValue;

        /// <summary>
        /// The registry key name.
        /// </summary>
        private readonly string _vanillaRegistryValuesFilePath;

        /// <summary>
        /// The registry manager
        /// </summary>
        private readonly ITrisoftRegistryManager _registryManager;

        /// <summary>
        /// The registry manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetRegistryValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="registryValue">The registry value.</param>
        /// <param name="vanillaRegistryValuesFilePath">The value.</param>
        public SetRegistryValueAction(ILogger logger, RegistryValue registryValue, string vanillaRegistryValuesFilePath = "")
            : base(logger)
        {
            _registryValue = registryValue;
            _vanillaRegistryValuesFilePath = vanillaRegistryValuesFilePath;

            _registryManager = ObjectFactory.GetInstance<ITrisoftRegistryManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        public override void Execute()
        {
            if (!string.IsNullOrEmpty(_vanillaRegistryValuesFilePath))
            {
                var registryValuesCollection = _fileManager.FileExists(_vanillaRegistryValuesFilePath) ?
                    _fileManager.ReadObjectFromFile<RegistryValueCollection>(_vanillaRegistryValuesFilePath) : 
                    new RegistryValueCollection();

                var vanillaValue = registryValuesCollection[_registryValue.Key, _registryValue.ValueName];
                if (vanillaValue == null)
                {
                    var currentValue = _registryManager.GetRegistryValue(_registryValue.Key, _registryValue.ValueName);
                    registryValuesCollection.Values.Add(new RegistryValue { Key = _registryValue.Key, ValueName = _registryValue.ValueName, Value = currentValue });
                    _fileManager.SaveObjectToFile(_vanillaRegistryValuesFilePath, registryValuesCollection);
                }
            }

            _registryManager.SetValue(_registryValue.Key, _registryValue.ValueName, _registryValue.Value);
        }
	}
}
