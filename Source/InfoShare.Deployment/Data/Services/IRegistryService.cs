using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace InfoShare.Deployment.Data.Services
{
    public interface IRegistryService
    {
        IEnumerable<RegistryKey> GetInstalledProjectsKeys();

        string GetInstallParamFilePath(RegistryKey projectRegKey);

        Version GetInstalledProjectVersion(RegistryKey projectRegKey);
    }
}
