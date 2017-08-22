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
﻿using System;
using System.Collections.Generic;
using System.Linq;
﻿using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
﻿using ISHDeploy.Common.Models.Backup;
﻿using Microsoft.Win32;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Manager that do all kinds of operations with registry.
    /// </summary>
    /// <seealso cref="ITrisoftRegistryManager" />
    public class TrisoftRegistryManager : ITrisoftRegistryManager
    {
        #region Private constants

        /// <summary>
        /// The project base registry name.
        /// </summary>
        private const string ProjectBaseRegName = "InfoShare";

        /// <summary>
        /// The core registry key name.
        /// </summary>
        private const string CoreRegName = "Core";

        /// <summary>
        /// The current registry key name.
        /// </summary>
        private const string CurrentRegName = "Current";

        /// <summary>
        /// The history registry key name.
        /// </summary>
        private const string HistoryRegName = "History";

        /// <summary>
        /// The install history path registry value name.
        /// </summary>
        private const string InstallHistoryPathRegValue = "InstallHistoryPath";

        /// <summary>
        /// The version registry value name
        /// </summary>
        private const string VersionRegValue = "Version";

        /// <summary>
        /// The ISHDeploymentStatus registry value name
        /// </summary>
        private const string ISHDeploymentStatusRegValue = "ISHDeploymentStatus";

        #endregion

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The path to of "SOFTWARE\Trisoft"
        /// </summary>
        public string RelativeTrisoftRegPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrisoftRegistryManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TrisoftRegistryManager(ILogger logger)
        {
            _logger = logger;
            RelativeTrisoftRegPath = Environment.Is64BitOperatingSystem ? "SOFTWARE\\Wow6432Node\\Trisoft" : "SOFTWARE\\Trisoft";
        }

        /// <summary>
        /// Gets the installed deployments keys.
        /// </summary>
        /// <param name="projectName">The deployment name. If not specified method will return all found deployments.</param>
        /// <returns>List of found deployments</returns>
        public IEnumerable<RegistryKey> GetInstalledProjectsKeys(string projectName = null)
        {
            _logger.WriteDebug("Retrieve registry keys", (string.IsNullOrEmpty(projectName) ? "for all installed projects" : projectName));

            var installedProjectsKeys = new List<RegistryKey>();
            var installToolRegKey = Registry.LocalMachine.OpenSubKey($@"{RelativeTrisoftRegPath}\InstallTool");

            var projectBaseRegKey = installToolRegKey?.OpenSubKey(ProjectBaseRegName);

            if (projectBaseRegKey == null || projectBaseRegKey.SubKeyCount == 0)
            {
                _logger.WriteWarning("None project base registry keys were found on the system.");
                return installedProjectsKeys;
            }

            string[] projectsKeyNames = projectBaseRegKey.GetSubKeyNames();

            foreach (var name in projectsKeyNames)
            {
                if (name == CoreRegName || (!string.IsNullOrEmpty(projectName) && name != projectName))
                {
                    continue;
                }

                var projRegKey = projectBaseRegKey.OpenSubKey(name);

                var currentValue = projRegKey?.GetValue(CurrentRegName, string.Empty).ToString();

                if (!string.IsNullOrWhiteSpace(currentValue))
                {
                    installedProjectsKeys.Add(projRegKey);
                }
            }

            _logger.WriteVerbose($"Registry keys for {(string.IsNullOrEmpty(projectName) ? "installed projects" : $"project `{projectName}`")} have been retrieved");
            return installedProjectsKeys;
        }

        /// <summary>
        /// Gets the status of deployment.
        /// </summary>
        /// <param name="projectSuffix">The project suffix.</param>
        /// <returns>Status of deployments</returns>
        public ISHDeploymentStatus GetISHDeploymentStatus(string projectSuffix)
        {
            _logger.WriteDebug("Retrieve the status of deployment", projectSuffix);

            var ishDeploymentStatus = ISHDeploymentStatus.Started;

            var installProjectsRegKeys = GetInstalledProjectsKeys(projectSuffix);
            var projectRegKey = installProjectsRegKeys.SingleOrDefault(x => x.Name.Split('\\').Last() == projectSuffix);

            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            var status = historyItem?.GetValue(ISHDeploymentStatusRegValue);
            if (status != null)
            {
                _logger.WriteDebug("ISHDeployment status", status);
                ishDeploymentStatus = (ISHDeploymentStatus)Enum.Parse(typeof(ISHDeploymentStatus), status.ToString());
            }

            _logger.WriteVerbose($"The status of ISHDeployment is {ishDeploymentStatus}");

            return ishDeploymentStatus;
        }

        /// <summary>
        /// Saves the status of deployment.
        /// </summary>
        /// <param name="projectSuffix">The project suffix.</param>
        /// <param name="status">The status of deployment.</param>
        public void SaveISHDeploymentStatus(string projectSuffix, ISHDeploymentStatus status)
        {
            _logger.WriteDebug("Save the status of deployment", projectSuffix);

            var installProjectsRegKeys = GetInstalledProjectsKeys(projectSuffix);
            var projectRegKey = installProjectsRegKeys.SingleOrDefault(x => x.Name.Split('\\').Last() == projectSuffix);
            var historyItem = GetHistoryFolderRegKey(projectRegKey, true);
            Registry.SetValue(historyItem.Name, ISHDeploymentStatusRegValue, status);
        }

        /// <summary>
        /// Removes the status of deployment from Registry.
        /// </summary>
        /// <param name="projectSuffix">The project suffix.</param>
        public void RemoveISHDeploymentStatus(string projectSuffix)
        {
            _logger.WriteDebug("Remove the status of deployment from Registry", projectSuffix);

            var installProjectsRegKeys = GetInstalledProjectsKeys(projectSuffix);
            var projectRegKey = installProjectsRegKeys.SingleOrDefault(x => x.Name.Split('\\').Last() == projectSuffix);
            var historyItem = GetHistoryFolderRegKey(projectRegKey, true);
            if (historyItem.GetValue(ISHDeploymentStatusRegValue) != null)
            {
                historyItem.DeleteValue(ISHDeploymentStatusRegValue, false);
            }
        }

        /// <summary>
        /// Gets the inputparameters.xml file path.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Path to inputparameters.xml file</returns>
        public string GetInstallParamFilePath(RegistryKey projectRegKey)
        {
            _logger.WriteDebug("[Get path to inputparameters.xml]");

            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            var path = historyItem?.GetValue(InstallHistoryPathRegValue).ToString();
            _logger.WriteVerbose($"The path to file inputparameters.xml for {projectRegKey.Name} has been retrieved");
            return path;
        }

        /// <summary>
        /// Gets the installed deployment version.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Deployment version.</returns>
        public Version GetInstalledProjectVersion(RegistryKey projectRegKey)
        {
            _logger.WriteDebug("[Get deployment version]");

            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            var versionStr = historyItem?.GetValue(VersionRegValue).ToString();
            Version version;

            if (string.IsNullOrWhiteSpace(versionStr) || !Version.TryParse(versionStr, out version))
            {
                _logger.WriteWarning($"`{projectRegKey}` registry key does not contain correct `{VersionRegValue}` value");
                return null;
            }

            _logger.WriteVerbose($"The version for installed deployment {projectRegKey.Name} has been retrieved");
            return version;
        }

        /// <summary>
        /// Gets the history folder registry key.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <param name="writable">Is registry key writable. Fales by default</param>
        /// <returns>Registry history folder key.</returns>
        private RegistryKey GetHistoryFolderRegKey(RegistryKey projectRegKey, bool writable = false)
        {
            var currentInstallvalue = projectRegKey?.GetValue(CurrentRegName).ToString();

            if (string.IsNullOrWhiteSpace(currentInstallvalue))
            {
                _logger.WriteWarning($"`{projectRegKey}` does not contain `{CurrentRegName}` key");
                return null;
            }

            var historyRegKey = projectRegKey.OpenSubKey(HistoryRegName);

            var installFolderRegKey = historyRegKey?.GetSubKeyNames().FirstOrDefault(keyName => keyName == currentInstallvalue);

            if (installFolderRegKey == null)
            {
                _logger.WriteWarning($"`{projectRegKey}` does not contain `{HistoryRegName}` key that named `{currentInstallvalue}`");
                return null;
            }

            return historyRegKey.OpenSubKey(installFolderRegKey, writable);
        }

        /// <summary>
        /// Gets the specified value on the specified registry key.
        /// </summary>
        /// <param name="keyName">The registry key name.</param>
        /// <param name="valueName">The name of value.</param>
        public object GetRegistryValue(string keyName, RegistryValueName valueName)
        {
            _logger.WriteDebug("Get registry value", keyName, valueName);
            var value = Registry.GetValue(keyName, valueName.ToString(), null);
            _logger.WriteVerbose($"The registry value `{keyName}\\{valueName}` is `{value}`");
            return value;
        }

        /// <summary>
        /// Gets names of all values of registry key.
        /// </summary>
        /// <param name="localMachineSubKeyName">The registry path of the sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <returns>
        /// The array of names of all values of specified registry key.
        /// </returns>
        public string[] GetValueNames(string localMachineSubKeyName)
        {
            _logger.WriteDebug("Get names of all values of registry key", localMachineSubKeyName);
            var result = Registry.LocalMachine.OpenSubKey(localMachineSubKeyName).GetValueNames();
            _logger.WriteVerbose($"The names of values for registry key `HKEY_LOCAL_MACHINE\\{localMachineSubKeyName}` has been loaded`");
            return result;
        }

        /// <summary>
        /// Copies values from one registry key to another.
        /// </summary>
        /// <param name="namesOfValues">The list of names of values that need to be copied.</param>
        /// <param name="sourceLocalMachineSubKeyName">The registry path to source sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <param name="destLocalMachineSubKeyName">The registry path to destination sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        public void CopyValues(IEnumerable<string> namesOfValues, string sourceLocalMachineSubKeyName, string destLocalMachineSubKeyName)
        {
            _logger.WriteDebug($"Copies values from `{sourceLocalMachineSubKeyName}` registry key to `{destLocalMachineSubKeyName}`");

            var sourceKey = Registry.LocalMachine.OpenSubKey(sourceLocalMachineSubKeyName);
            var destKey = Registry.LocalMachine.OpenSubKey(destLocalMachineSubKeyName, true);
            foreach (var nameOfValue in namesOfValues)
            {
                destKey.SetValue(nameOfValue, sourceKey.GetValue(nameOfValue));
            }
            _logger.WriteVerbose($"The values from `{sourceLocalMachineSubKeyName}` registry has been copied to `{destLocalMachineSubKeyName}`");
        }

        /// <summary>
        /// Gets properties from one registry key
        /// </summary>
        /// <param name="namesOfValues">The list of names of values that need to be copied.</param>
        /// <param name="sourceLocalMachineSubKeyName">The registry path to source sub key under LocalMachine (HKEY_LOCAL_MACHINE).</param>
        /// <returns>
        /// Properties from one registry key.
        /// </returns>
        public PropertyCollection GetValues(IEnumerable<string> namesOfValues, string sourceLocalMachineSubKeyName)
        {
            _logger.WriteDebug($"Gets values from `{sourceLocalMachineSubKeyName}` registry key");

            var result = new PropertyCollection();
            var sourceKey = Registry.LocalMachine.OpenSubKey(sourceLocalMachineSubKeyName);
            foreach (var nameOfValue in namesOfValues)
            {
                var value = sourceKey.GetValue(nameOfValue);
                result.Properties.Add(new Property { Name = nameOfValue, Value = value});
            }
            _logger.WriteVerbose($"The values from `{sourceLocalMachineSubKeyName}` registry has been got");
            return result;
        }

        /// <summary>
        /// Sets value in registry key
        /// </summary>
        /// <param name="keyName">The registry key name.</param>
        /// <param name="valueName">The name of value.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string keyName, RegistryValueName valueName, object value)
        {
            SetValue(keyName, valueName.ToString(), value);
        }

        /// <summary>
        /// Sets value in registry key
        /// </summary>
        /// <param name="keyName">The registry key name.</param>
        /// <param name="valueName">The name of value.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string keyName, string valueName, object value)
        {
            _logger.WriteDebug($"Sets value to `{keyName}` registry key");

            Registry.SetValue(keyName, valueName, value);
        }
    }
}
