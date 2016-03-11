using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Data.Exceptions;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.ISHProject
{
    /// <summary>
    /// Gets all instances of the installed Content Manager deployment for the current system.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class GetISHDeploymentsAction : BaseActionWithResult<IEnumerable<ISHDeployment>>
    {
        /// <summary>
        /// The input parameters file name
        /// </summary>
        private const string InputParametersFileName = "inputparameters.xml";

        /// <summary>
        /// The registry manager.
        /// </summary>
        private readonly IRegistryManager _registryManager;

        /// <summary>
        /// The XML configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The Content Manager deployment suffix.
        /// </summary>
        private readonly string _projectSuffix;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="projectSuffix">The Content Manager deployment suffix.</param>
        /// <param name="returnResult">The delegate that returns list of Content Manager deployments.</param>
        public GetISHDeploymentsAction(ILogger logger, string projectSuffix, Action<IEnumerable<ISHDeployment>> returnResult)
            : base(logger, returnResult)
        {
            _registryManager = ObjectFactory.GetInstance<IRegistryManager>();
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _projectSuffix = projectSuffix;
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>List of Content Manager deployments.</returns>
        protected override IEnumerable<ISHDeployment> ExecuteWithResult()
        {
            var result = new List<ISHDeployment>();

            // Get list of installed deployments from the registry.
            var installProjectsRegKeys = _registryManager.GetInstalledProjectsKeys(_projectSuffix).ToArray();
            
            if (!installProjectsRegKeys.Any())
            {
                if (_projectSuffix != null)
                {
                    Logger.WriteError(
                        new DeploymentNotFoundException(
                            $"Deployment with suffix {_projectSuffix} is not found on the system"), _projectSuffix);
                }
                else
                {
                    Logger.WriteVerbose("None project instances were found on the system");
                }

                return result;
            }

            // For each registry record get deployment version, path to inputparameter.xml file and parse this file.
            foreach (var projectRegKey in installProjectsRegKeys)
            {
                var installParamsPath = _registryManager.GetInstallParamFilePath(projectRegKey);
                var version = _registryManager.GetInstalledProjectVersion(projectRegKey);

                if (string.IsNullOrWhiteSpace(installParamsPath))
                {
                    Logger.WriteError(new CorruptedInstallationException($"Registry subkeys for {projectRegKey} are corrupted"), projectRegKey);
                    continue;
                }

                var installParamFile = Path.Combine(installParamsPath, InputParametersFileName);

                if (!_fileManager.Exists(installParamFile))
                {
                    Logger.WriteError(new CorruptedInstallationException($"{ installParamFile } file does not exist on the system"), installParamFile);
                    continue;
                }

                var dictionary = _xmlConfigManager.GetAllInputParamsValues(installParamFile);

                var ishProject = new ISHDeployment(dictionary, version);

                result.Add(ishProject);
            }

            return result;
        }
    }
}
