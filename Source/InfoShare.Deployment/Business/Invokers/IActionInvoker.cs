using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Invokers
{
    /// <summary>
    /// Executes the sequence of actions one by one
    /// </summary>
    public interface IActionInvoker
    {
        /// <summary>
		/// Adds Action into invocation list
        /// </summary>
		/// <param name="action">An action to invoce <see cref="T:InfoShare.Deployment.Interfaces.Actions.IAction"/>.</param>
        void AddAction(IAction action);

        /// <summary>
		/// Invokes actions sequence execution
        /// </summary>
        void Invoke();
    }
}
