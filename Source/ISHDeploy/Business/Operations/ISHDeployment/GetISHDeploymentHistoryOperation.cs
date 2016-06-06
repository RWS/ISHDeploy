using ISHDeploy.Interfaces;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Gets history file content.
    /// </summary>
    /// <seealso cref="IOperation{TResult}" />
    public class GetISHDeploymentHistoryOperation : BasePathsOperation, IOperation<string>
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The history file content
        /// </summary>
        private string _historyContent = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentsOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
        public GetISHDeploymentHistoryOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Getting the history file content");

            _invoker.AddAction(new FileReadAllTextAction(logger, HistoryFilePath, result => _historyContent = result));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        /// <returns>List of installed Content Manager deployments.</returns>
        public string Run()
        {
            _invoker.Invoke();

            return _historyContent;
        }
    }
}
