using System;
using System.Collections.Generic;
using System.IO;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Commands.ISHProjectCommands
{
    public class GetISHProjectsCommand : BaseCommandWithResult<IEnumerable<ISHProject>>
    {
        private readonly RegistryService _registryService;
        private readonly XmlConfigManager _xmlConfigManager;

        public GetISHProjectsCommand(ILogger logger, Action<IEnumerable<ISHProject>> getResult)
            : base(logger, getResult)
        {
            _registryService = new RegistryService(logger);
            _xmlConfigManager = new XmlConfigManager(Logger);
        }

        protected override IEnumerable<ISHProject> ExecuteWithResult()
        {
            var result = new List<ISHProject>();

            var installProjectsRegKeys = _registryService.GetInstalledProjectsKeys();
            
            foreach (var projectRegKey in installProjectsRegKeys)
            {
                var installParamsPath = _registryService.GetInstallParamFilePath(projectRegKey);
                var version = _registryService.GetInstalledProjectVersion(projectRegKey);

                var installParamFile = Path.Combine(installParamsPath, ISHPaths.InputParametersFile);

                if (!File.Exists(installParamFile))
                {
                    Logger.WriteWarning($"{installParamFile} cannot be found");
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
