using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;
using ISHDeploy.Models.ISHXmlNodes;

namespace ISHDeploy.Business.Operations.ISHSTS
{
	/// <summary>
	/// Sets Event Monitor Tab.
	/// </summary>
	/// <seealso cref="ISHDeploy.Business.Operations.IOperation" />
	public class SetISHIntegrationSTSCertificateOperation : OperationPaths, IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="paths">Reference for all files paths.</param>
		/// <param name="menuItem">The menu item object.</param>
		public SetISHIntegrationSTSCertificateOperation(ILogger logger, IssuerThumbprintItem menuItem)
		{
			_invoker = new ActionInvoker(logger, "Setting of Event Monitor Tab");

			_invoker.AddAction(new SetNodeAction(logger, AuthorAspWebConfig.Path, String.Format(AuthorAspWebConfig.STSIdentityTrustedIssuers, menuItem.Thumbprint), menuItem, false));
			_invoker.AddAction(new SetNodeAction(logger, InfoShareWSWebConfig.Path, String.Format(InfoShareWSWebConfig.STSIdentityTrustedIssuers, menuItem.Thumbprint), menuItem, false));
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
