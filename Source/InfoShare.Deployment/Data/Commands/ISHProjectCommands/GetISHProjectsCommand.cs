using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Exceptions;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Commands.ISHProjectCommands
{
    public class GetISHProjectsCommand : BaseCommandWithResult<IEnumerable<ISHProject>>
    {
        private readonly IRegistryService _registryService;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly IFileManager _fileManager;
        private readonly string _projectSuffix;

        public GetISHProjectsCommand(ILogger logger, string projectSuffix, Action<IEnumerable<ISHProject>> returnResult)
            : base(logger, returnResult)
        {
            _registryService = ObjectFactory.GetInstance<IRegistryService>();
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _projectSuffix = projectSuffix;
        }

        protected override IEnumerable<ISHProject> ExecuteWithResult()
        {
            var result = new List<ISHProject>();

            var installProjectsRegKeys = _registryService.GetInstalledProjectsKeys(_projectSuffix).ToArray();
            
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
                var installParamsPath = _registryService.GetInstallParamFilePath(projectRegKey);
                var version = _registryService.GetInstalledProjectVersion(projectRegKey);

                if (string.IsNullOrWhiteSpace(installParamsPath))
                {
                    Logger.WriteError(new CorruptedInstallationException($"Registry subkeys for {projectRegKey} are corrupted"), projectRegKey);
                    continue;
                }

                var installParamFile = Path.Combine(installParamsPath, ISHPaths.InputParametersFile);

                if (!_fileManager.Exists(installParamFile))
                {
                    Logger.WriteError(new CorruptedInstallationException($"{ installParamFile } file does not exist on the system"), installParamFile);
                    continue;
                }

                var dictionary = _xmlConfigManager.GetAllInstallParamsValues(installParamFile);

                var ishProject = new ISHProject(dictionary, version);

                result.Add(ishProject);
            }

            return result;
        }
    }
}
