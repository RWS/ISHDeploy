using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations.ISHUIEventMonitorTab
{
	/// <summary>
	/// Sets Event Monitor Tab.
	/// </summary>
	/// <seealso cref="ISHDeploy.Business.Operations.IOperation" />
	public class SetISHUIEventMonitorTabOperation : IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveISHUIEventMonitorTabOperation" /> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="paths">Reference for all files paths.</param>
		/// <param name="menuItem">The menu item object.</param>
		public SetISHUIEventMonitorTabOperation(ILogger logger, ISHPaths paths, EventLogMenuItem menuItem)
		{
			_invoker = new ActionInvoker(logger, "Setting of Event Monitor Tab");

			_invoker.AddAction(new SetNodeAction(logger, paths.EventMonitorMenuBar, String.Format(CommentPatterns.EventMonitorTab, menuItem.Label), menuItem));
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
