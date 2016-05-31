using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Manager that do all kinds of operations with registry.
    /// </summary>
    public interface IRegistryManager
    {
        /// <summary>
        /// Gets the installed deployments keys.
        /// </summary>
        /// <param name="expectedSuffix">The deployment suffix. If not specified method will return all found deployments.</param>
        /// <returns>List of found deployments</returns>
        IEnumerable<RegistryKey> GetInstalledProjectsKeys(string expectedSuffix = null);

                /// <summary>
        /// Gets the inputparameters.xml file path.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Path to inputparameters.xml file</returns>
        string GetInstallParamFilePath(RegistryKey projectRegKey);

        /// <summary>
        /// Gets the installed deployment version.
        /// </summary>
        /// <param name="projectRegKey">The deployment registry key.</param>
        /// <returns>Deployment version.</returns>
        Version GetInstalledProjectVersion(RegistryKey projectRegKey);
    }
}
