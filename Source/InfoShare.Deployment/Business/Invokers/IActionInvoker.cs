using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Invokers
{
    /// <summary>
    /// Executes the sequence of actions one by one
    /// </summary>
    public interface IActionInvoker
    {
        /// <summary>
        /// Adds action to the sequence
        /// </summary>
        /// <param name="action">New action to the sequence</param>
        void AddAction(IAction action);

        /// <summary>
        /// Executes sequence of actions one by one
        /// </summary>
        void Invoke();
    }
}
