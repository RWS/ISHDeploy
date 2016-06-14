using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Adds entry to the history file about cmdlets usage
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class AddHistoryEntryOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHistoryEntryOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="text">The text with description which cmdlet was executed with which parameters.</param>
        public AddHistoryEntryOperation(ILogger logger, Models.ISHDeployment ishDeployment, string text) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Adding of entry to the history file about cmdlets usage");
            _invoker.AddAction(new FileAddHistoryEntryAction(logger, HistoryFilePath, text, ishDeployment.Name));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
