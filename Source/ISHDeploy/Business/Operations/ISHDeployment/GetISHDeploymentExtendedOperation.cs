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
    public class GetISHDeploymentExtendedOperation : IOperation<Models.ISHDeploymentExtended>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The list of installed Content Manager deployments found on the current system.
        /// </summary>
        private Models.ISHDeploymentExtended _ishProject;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentExtendedOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="name">The deployment name.</param>
        public GetISHDeploymentExtendedOperation(ILogger logger, string name)
        {
            _invoker = new ActionInvoker(logger, "Get a list of installed Content Manager deployments");
            _invoker.AddAction(new GetISHDeploymentExtendedAction(logger, name, result => _ishProject = result));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public Models.ISHDeploymentExtended Run()
        {
            _invoker.Invoke();

            return _ishProject;
        }
    }
}
