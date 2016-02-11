using System;
using System.Collections.Generic;
using System.Linq;
using InfoShare.Deployment.Interfaces;
using Microsoft.Win32;

namespace InfoShare.Deployment.Data.Services
{
    public class RegistryService : IRegistryService
    {
        #region Private constants

        private const string InstallToolRegPath = "SOFTWARE\\Trisoft\\InstallTool";
        private const string InstallToolRegPath64 = "SOFTWARE\\Wow6432Node\\Trisoft\\InstallTool";
        private const string ProjectBaseRegName = "InfoShare";
        private const string CoreRegName = "Core";
        private const string CurrentRegName = "Current";
        private const string HistoryRegName = "History";
        private const string InstallHistoryPathRegValue = "InstallHistoryPath";
        private const string VersionRegValue = "Version";

        #endregion

        private readonly ILogger _logger;

        public RegistryService(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<RegistryKey> GetInstalledProjectsKeys()
        {
            var projectBaseRegKey = GetProjectBaseRegKey();

            if (projectBaseRegKey == null || projectBaseRegKey.SubKeyCount == 0)
            {
                _logger.WriteVerbose("None project instances were found in the registry");
                return null;
            }

            string[] projectsKeyNames = projectBaseRegKey.GetSubKeyNames();
            var installedProjectsKeys = new List<RegistryKey>();

            foreach (var projectName in projectsKeyNames)
            {
                if (projectName != CoreRegName)
                {
                    var projRegKey = projectBaseRegKey.OpenSubKey(projectName);
                    
                    var currentValue = projRegKey?.GetValue(CurrentRegName, string.Empty).ToString();

                    if (!string.IsNullOrWhiteSpace(currentValue))
                    {
                        installedProjectsKeys.Add(projRegKey);
                    }
                }
            }

            return installedProjectsKeys;
        }

        public string GetInstallParamFilePath(RegistryKey projectRegKey)
        {
            var historyItem = GetHistoryFolderRegKey(projectRegKey);

            return historyItem?.GetValue(InstallHistoryPathRegValue).ToString();
        }

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
    }
}
