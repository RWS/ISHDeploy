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
    public class GetISHDeploymentsAction : BaseActionWithResult<IEnumerable<ISHDeployment>>
    {
        private readonly IRegistryManager _registryManager;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly IFileManager _fileManager;
        private readonly string _projectSuffix;
        private const string InputParametersFileName = "inputparameters.xml";

        public GetISHDeploymentsAction(ILogger logger, string projectSuffix, Action<IEnumerable<ISHDeployment>> returnResult)
            : base(logger, returnResult)
        {
            _registryManager = ObjectFactory.GetInstance<IRegistryManager>();
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _projectSuffix = projectSuffix;
        }

        protected override IEnumerable<ISHDeployment> ExecuteWithResult()
        {
            var result = new List<ISHDeployment>();

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

                var dictionary = _xmlConfigManager.GetAllInstallParamsValues(installParamFile);

                var ishProject = new ISHDeployment(dictionary, version);

                result.Add(ishProject);
            }

            return result;
        }
    }
}
