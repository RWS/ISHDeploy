using System;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIEventMonitorTab
{
	/// <summary>
	/// Removes Event Monitor Tab".
	/// </summary>
	/// <seealso cref="IOperation" />
	public class RemoveISHUIEventMonitorTabOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveISHUIEventMonitorTabOperation"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="paths">Reference for all files paths.</param>
		/// <param name="label">Label of the element</param>
		public RemoveISHUIEventMonitorTabOperation(ILogger logger, ISHPaths paths, string label)
        {
            _invoker = new ActionInvoker(logger, "Removing Event Monitor Tab");

			string itemXPath = String.Format(CommentPatterns.EventMonitorTab, label);
			string itemCommentXPath = itemXPath + CommentPatterns.EventMonitorPreccedingCommentXPath;

			// First we should remove comment as it is dependent to its sibling node
			_invoker.AddAction(new RemoveSingleNodeAction(logger, paths.EventMonitorMenuBar, itemCommentXPath));

			// Then we removing item itself
			_invoker.AddAction(new RemoveSingleNodeAction(logger, paths.EventMonitorMenuBar, itemXPath));
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
