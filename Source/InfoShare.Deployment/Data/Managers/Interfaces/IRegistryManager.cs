using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace InfoShare.Deployment.Data.Managers.Interfaces
{
    public interface IRegistryManager
    {
        IEnumerable<RegistryKey> GetInstalledProjectsKeys(string expectedSuffix = null);

        string GetInstallParamFilePath(RegistryKey projectRegKey);

        Version GetInstalledProjectVersion(RegistryKey projectRegKey);
    }
}
