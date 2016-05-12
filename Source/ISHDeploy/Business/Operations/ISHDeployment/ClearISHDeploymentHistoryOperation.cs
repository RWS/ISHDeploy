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
        public ClearISHDeploymentHistoryOperation(ILogger logger)
        {
            _invoker = new ActionInvoker(logger, "Customization history clean up");

            _invoker.AddAction(new FileDeleteAction(logger, OperationPaths.HistoryFilePath));
            _invoker.AddAction(new FileCleanDirectoryAction(logger, OperationPaths.FoldersPaths.BackupFolderPath));
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
