using System.Collections.Generic;
using System.Linq;
using InfoShare.Deployment.Data.Commands.ISHProjectCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHDeployment
{
    public class GetISHDeploymentCmdSet : ICmdSet<IEnumerable<ISHProject>>
    {
        private readonly ICommand _command;
        private readonly string _deploySuffix;

        private IEnumerable<ISHProject> _ishProjects;

        public GetISHDeploymentCmdSet(ILogger logger, string deploySuffix)
        {
            _deploySuffix = deploySuffix;
            _command = new GetISHProjectsCommand(logger, result => _ishProjects = result);
        }

        public IEnumerable<ISHProject> Run()
        {
            _command.Execute();

            if (!string.IsNullOrWhiteSpace(_deploySuffix))
            {
                return _ishProjects.Where(ishProject => ishProject.Suffix == _deploySuffix);
            }

            return _ishProjects;
        }
    }
}
