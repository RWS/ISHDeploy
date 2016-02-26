using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Invokers
{
    public interface IActionInvoker
    {
		/// <summary>
		/// Adds Action into invocation list
		/// </summary>
		/// <param name="action">An action to invoce <see cref="T:InfoShare.Deployment.Interfaces.Actions.IAction"/>.</param>
		void AddAction(IAction action);

		/// <summary>
		/// Invokes actions execution
		/// </summary>
        void Invoke(bool isRollbackAllowed);
    }
}
