using System.Collections.Generic;
using InfoShare.Deployment.Data.Actions.ISHProject;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Operations.ISHDeployment
{
    public class GetISHDeploymentOperation : IOperation<IEnumerable<Models.ISHDeployment>>
    {
        private readonly IAction _action;

        private IEnumerable<Models.ISHDeployment> _ishProjects;

        public GetISHDeploymentOperation(ILogger logger, string projectSuffix)
        {
            _action = new GetISHDeploymentsAction(logger, projectSuffix, result => _ishProjects = result);
        }

        public IEnumerable<Models.ISHDeployment> Run()
        {
            _action.Execute();

            return _ishProjects;
        }
    }
}
