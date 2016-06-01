using System.Collections.Generic;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Interfaces;
using ISHDeploy.Business.Invokers;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Gets a list of installed Content Manager deployments found on the current system.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHDeploymentsOperations : IOperation<IEnumerable<Models.ISHDeployment>>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The list of installed Content Manager deployments found on the current system.
        /// </summary>
        private IEnumerable<Models.ISHDeployment> _ishProjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentsOperations"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="projectName">The deployment name.</param>
        public GetISHDeploymentsOperations(ILogger logger, string projectName)
        {
            _invoker = new ActionInvoker(logger, "Getting a list of installed Content Manager deployments");
            _invoker.AddAction(new GetISHDeploymentsAction(logger, projectName, result => _ishProjects = result));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public IEnumerable<Models.ISHDeployment> Run()
        {
            _invoker.Invoke();

            return _ishProjects;
        }
    }
}
