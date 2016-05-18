using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIEventMonitorTab
{
	/// <summary>
	/// Removes Event Monitor Tab".
	/// </summary>
	/// <seealso cref="IOperation" />
	public class RemoveISHUIEventMonitorTabOperation : OperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveISHUIEventMonitorTabOperation"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="label">Label of the element</param>
		public RemoveISHUIEventMonitorTabOperation(ILogger logger, string label)
        {
            _invoker = new ActionInvoker(logger, "Removing of Event Monitor Tab");

			string itemXPath = string.Format(EventMonitorMenuBarXml.EventMonitorTab, label);
			string itemCommentXPath = itemXPath + EventMonitorMenuBarXml.EventMonitorPreccedingCommentXPath;

			// First we should remove comment as it is dependent to its sibling node
			_invoker.AddAction(new RemoveSingleNodeAction(logger, EventMonitorMenuBarXml.Path, itemCommentXPath));

			// Then we removing item itself
			_invoker.AddAction(new RemoveSingleNodeAction(logger, EventMonitorMenuBarXml.Path, itemXPath));
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
