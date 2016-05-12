using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIEventMonitorTab
{
	/// <summary>
	/// Moves Event Monitor Tab".
	/// </summary>
	/// <seealso cref="IOperation" />
	public class MoveISHUIEventMonitorTabOperation : OperationPaths, IOperation
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
		/// <param name="label">Label of the element</param>
		/// <param name="operationType">Type of the operation.</param>
		/// <param name="targetLabel">The target label.</param>
		public MoveISHUIEventMonitorTabOperation(ILogger logger, string label, OperationType operationType, string targetLabel = null)
        {
            _invoker = new ActionInvoker(logger, "Moving of Event Monitor Tab");

			string nodeXPath = string.Format(EventMonitorMenuBarXml.EventMonitorTab, label);
			string nodeCommentXPath = nodeXPath + EventMonitorMenuBarXml.EventMonitorPreccedingCommentXPath;

			string targetNodeXPath = string.IsNullOrEmpty(targetLabel) ? null : string.Format(EventMonitorMenuBarXml.EventMonitorTab, targetLabel);

			// Combile node and its xPath
			string nodesToMoveXPath = nodeXPath + "|" + nodeCommentXPath;

			switch (operationType)
	        {
				case OperationType.InsertAfter:
					_invoker.AddAction(new MoveAfterNodeAction(logger, EventMonitorMenuBarXml.Path, nodesToMoveXPath, targetNodeXPath));
					break;
				case OperationType.InsertBefore:
					_invoker.AddAction(new MoveBeforeNodeAction(logger, EventMonitorMenuBarXml.Path, nodesToMoveXPath, targetNodeXPath));
					break;
			}
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
