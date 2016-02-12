using System.Collections.Generic;
using InfoShare.Deployment.Data.Commands.ISHProjectCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Business.CmdSets.ISHDeployment
{
    public class GetISHDeploymentCmdSet : ICmdSet<IEnumerable<Models.ISHDeployment>>
    {
        private readonly ICommand _command;

        private IEnumerable<Models.ISHDeployment> _ishProjects;

        public GetISHDeploymentCmdSet(ILogger logger, string projectSuffix)
        {
            _command = new GetISHDeploymentsCommand(logger, projectSuffix, result => _ishProjects = result);
        }

        public IEnumerable<Models.ISHDeployment> Run()
        {
            _command.Execute();

            return _ishProjects;
        }
    }
}
