using System.Collections.Generic;
using InfoShare.Deployment.Data.Commands.ISHProjectCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHDeployment
{
    public class GetISHDeploymentCmdSet : ICmdSet<IEnumerable<ISHProject>>
    {
        private readonly ICommand _command;

        private IEnumerable<ISHProject> _ishProjects;

        public GetISHDeploymentCmdSet(ILogger logger, string projectSuffix)
        {
            _command = new GetISHProjectsCommand(logger, projectSuffix, result => _ishProjects = result);
        }

        public IEnumerable<ISHProject> Run()
        {
            _command.Execute();

            return _ishProjects;
        }
    }
}
