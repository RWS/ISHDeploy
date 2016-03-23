using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions
{
    /// <summary>
    /// Base class for action that can be executed.
    /// </summary>
    /// <seealso cref="IAction" />
    public abstract class BaseAction : IAction
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BaseAction(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public abstract void Execute();
    }
}
