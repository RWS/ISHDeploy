using System;
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using Microsoft.Win32;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Manager that do all kinds of operations with registry.
    /// </summary>
    /// <seealso cref="IRegistryManager" />
    public class RegistryManager : IRegistryManager
    {
        #region Private constants

        /// <summary>
        /// The install tool registry path.
        /// </summary>
        private const string InstallToolRegPath = "SOFTWARE\\Trisoft\\InstallTool";

        /// <summary>
        /// The install tool registry path for x64 OS.
        /// </summary>
        private const string InstallToolRegPath64 = "SOFTWARE\\Wow6432Node\\Trisoft\\InstallTool";

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

        #endregion

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public RegistryManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the installed deployments keys.
        /// </summary>
        /// <param name="expectedSuffix">The deployment suffix. If not specified method will return all found deployments.</param>
        /// <returns>List of found deployments</returns>
        public IEnumerable<RegistryKey> GetInstalledProjectsKeys(string expectedSuffix = null)
        {
            var installedProjectsKeys = new List<RegistryKey>();
            var projectBaseRegKey = GetProjectBaseRegKey();

            if (projectBaseRegKey == null || projectBaseRegKey.SubKeyCount == 0)
            {
                _logger.WriteDebug("None project base registry keys were found on the system.");
                return installedProjectsKeys;
            }

            string[] projectsKeyNames = projectBaseRegKey.GetSubKeyNames();

            foreach (var projectName in projectsKeyNames)
            {
                if (projectName == CoreRegName || (expectedSuffix != null && expectedSuffix != GetProjectSuffix(projectName) ))
                {
                    continue;
                }

                var projRegKey = projectBaseRegKey.OpenSubKey(projectName);
                    
                var currentValue = projRegKey?.GetValue(CurrentRegName, string.Empty).ToString();

                if (!string.IsNullOrWhiteSpace(currentValue))
                {
                    installedProjectsKeys.Add(projRegKey);
                }
            }

            return installedProjectsKeys;
        }

        /// <summary>
        /// Gets the inputparameters.xml file path.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Path to inputparameters.xml file</returns>
        public string GetInstallParamFilePath(RegistryKey projectRegKey)
        {
            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            return historyItem?.GetValue(InstallHistoryPathRegValue).ToString();
        }

        /// <summary>
        /// Gets the installed deployment version.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Deployment version.</returns>
        public Version GetInstalledProjectVersion(RegistryKey projectRegKey)
        {
            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            var versionStr = historyItem?.GetValue(VersionRegValue).ToString();
            Version version;

            if (string.IsNullOrWhiteSpace(versionStr) || !Version.TryParse(versionStr, out version))
            {
                _logger.WriteDebug($"{projectRegKey} registry key does not contain correct {VersionRegValue} value");
                return null;
            }
            
            return version;
        }

        /// <summary>
        /// Gets the history folder registry key.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Registry history folder key.</returns>
        private RegistryKey GetHistoryFolderRegKey(RegistryKey projectRegKey)
        {
            var currentInstallvalue = projectRegKey?.GetValue(CurrentRegName).ToString();

            if (string.IsNullOrWhiteSpace(currentInstallvalue))
            {
                _logger.WriteDebug($"{projectRegKey} does not contain {CurrentRegName} key");
                return null;
            }

            var historyRegKey = projectRegKey.OpenSubKey(HistoryRegName);

            var installFolderRegKey = historyRegKey?.GetSubKeyNames().FirstOrDefault(keyName => keyName == currentInstallvalue);

            if (installFolderRegKey == null)
            {
                _logger.WriteDebug($"{projectRegKey} does not contain {HistoryRegName} key that named {currentInstallvalue}");
                return null;
            }

            return historyRegKey.OpenSubKey(installFolderRegKey);
        }

        /// <summary>
        /// Gets the deployment base registry key.
        /// </summary>
        /// <returns>Deployment base registry key.</returns>
        private RegistryKey GetProjectBaseRegKey()
        {
            RegistryKey installToolRegKey = null;

            if (Environment.Is64BitOperatingSystem)
            {
                _logger.WriteDebug($"Try to open registry key {InstallToolRegPath64}");
                installToolRegKey = Registry.LocalMachine.OpenSubKey(InstallToolRegPath64);
            }

            if (installToolRegKey == null)
            {
                _logger.WriteDebug($"Try to open registry key {InstallToolRegPath}");
                installToolRegKey = Registry.LocalMachine.OpenSubKey(InstallToolRegPath);
            }
            
            return installToolRegKey?.OpenSubKey(ProjectBaseRegName);
        }

        /// <summary>
        /// Gets the deployment suffix.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>Deployment suffix.</returns>
        private string GetProjectSuffix(string projectName)
        {
            if (projectName.Length < 9)
            {
                _logger.WriteWarning($"Unexpected project name in the registry: {projectName}");
                return null;
            }

            return projectName.Substring(9);
        }
    }
}
