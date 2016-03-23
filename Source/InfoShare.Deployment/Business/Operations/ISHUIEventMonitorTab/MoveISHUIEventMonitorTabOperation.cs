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
	public class MoveISHUIEventMonitorTabOperation : IOperation
    {
		/// <summary>
		/// Operation type enum
		/// </summary>
		public enum OperationType
		{

			/// <summary>
			/// Flag to insert after 
			/// </summary>
			InsertAfter,

			/// <summary>
			/// Flag to insert before
			/// </summary>
			InsertBefore
		}

        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveISHUIEventMonitorTabOperation" /> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="paths">Reference for all files paths.</param>
		/// <param name="label">Label of the element</param>
		/// <param name="operationType">Type of the operation.</param>
		/// <param name="targetLabel">The target label.</param>
		public MoveISHUIEventMonitorTabOperation(ILogger logger, ISHPaths paths, string label, OperationType operationType, string targetLabel = null)
        {
            _invoker = new ActionInvoker(logger, "Removing Event Monitor Tab");

			string nodeXPath = String.Format(CommentPatterns.EventMonitorTab, label);
			string targetNodeXPath = String.IsNullOrEmpty(targetLabel) ? null :  String.Format(CommentPatterns.EventMonitorTab, targetLabel);

			string itemCommentXPath = nodeXPath + CommentPatterns.EventMonitorPreccedingCommentXPath;
			string targetCommentXPath = targetNodeXPath + CommentPatterns.EventMonitorPreccedingCommentXPath;

			switch (operationType)
	        {
				case OperationType.InsertAfter:
					_invoker.AddAction(new InsertAfterNodeAction(logger, paths.EventMonitorMenuBar, nodeXPath, targetNodeXPath));
					break;
				case OperationType.InsertBefore:
					_invoker.AddAction(new InsertBeforeNodeAction(logger, paths.EventMonitorMenuBar, nodeXPath, targetNodeXPath));
					break;
			}

			// After node is moved, we also should move its comment
			_invoker.AddAction(new InsertBeforeNodeAction(logger, paths.EventMonitorMenuBar, itemCommentXPath, nodeXPath));
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
