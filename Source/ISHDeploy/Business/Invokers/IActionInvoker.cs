using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Business.Invokers
{
    /// <summary>
    /// Executes the sequence of actions one by one
    /// </summary>
    public interface IActionInvoker
    {
        /// <summary>
		/// Adds Action into invocation list
        /// </summary>
		/// <param name="action">An action to invoke <see cref="T:ISHDeploy.Interfaces.Actions.IAction"/>.</param>
        void AddAction(IAction action);

        /// <summary>
        /// Invokes actions sequence execution
        /// </summary>
        void Invoke();
    }
}
