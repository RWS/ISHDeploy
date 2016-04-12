using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
    /// <summary>
    /// Clears customization history for Content Manager deployment
    /// </summary>
    public class ClearISHDeploymentHistoryOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearISHDeploymentHistoryOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="historyFilePath">The path to history file.</param>
        /// <param name="backupFolderPath">The path to backup file.</param>
        public ClearISHDeploymentHistoryOperation(ILogger logger, string historyFilePath, string backupFolderPath)
        {
            _invoker = new ActionInvoker(logger, "Customization history clean up");

            _invoker.AddAction(new FileDeleteAction(logger, historyFilePath));
            _invoker.AddAction(new FileCleanDirectoryAction(logger, backupFolderPath));
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
