using System.Collections.Generic;
using InfoShare.Deployment.Data.Actions.ISHProject;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Gets the list of installed Content Manager deployments found on the current system.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHDeploymentOperation : IOperation<IEnumerable<Models.ISHDeployment>>
    {
        /// <summary>
        /// The action to be executed.
        /// </summary>
        private readonly IAction _action;

        /// <summary>
        /// The list of installed Content Manager deployments found on the current system.
        /// </summary>
        private IEnumerable<Models.ISHDeployment> _ishProjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="projectSuffix">The deployment suffix.</param>
        public GetISHDeploymentOperation(ILogger logger, string projectSuffix)
        {
            _action = new GetISHDeploymentsAction(logger, projectSuffix, result => _ishProjects = result);
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public IEnumerable<Models.ISHDeployment> Run()
        {
            _action.Execute();

            return _ishProjects;
        }
    }
}
