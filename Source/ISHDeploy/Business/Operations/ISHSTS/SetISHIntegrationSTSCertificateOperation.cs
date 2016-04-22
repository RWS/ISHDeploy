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
	public class SetISHIntegrationSTSCertificateOperation : IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		private readonly IActionInvoker _invoker;

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="menuItem">The menu item object.</param>
		public SetISHIntegrationSTSCertificateOperation(ILogger logger, IssuerThumbprintItem menuItem)
		{
			_invoker = new ActionInvoker(logger, "Setting of Event Monitor Tab");

			// Author web Config
			_invoker.AddAction(new SetNodeAction(logger, OperationPaths.InfoShareAuthorWebConfig.Path, 
				String.Format(OperationPaths.InfoShareAuthorWebConfig.IdentityTrustedIssuersPath, menuItem.Thumbprint), menuItem, false));

			_invoker.AddAction(new SetAttributeValueAction(logger, OperationPaths.InfoShareAuthorWebConfig.Path,
				OperationPaths.InfoShareAuthorWebConfig.CertificateValidationModePath, menuItem.ValidationMode.ToString()));

			// WS web Config
			_invoker.AddAction(new SetNodeAction(logger, OperationPaths.InfoShareWSWebConfig.Path, 
				String.Format(OperationPaths.InfoShareAuthorWebConfig.IdentityTrustedIssuersPath, menuItem.Thumbprint), menuItem, false));

			_invoker.AddAction(new SetAttributeValueAction(logger, OperationPaths.InfoShareWSWebConfig.Path,
				OperationPaths.InfoShareWSWebConfig.CertificateValidationModePath, menuItem.ValidationMode.ToString()));

			// STS web Config
			_invoker.AddAction(new SetNodeAction(logger, OperationPaths.InfoShareSTSWebConfig.Path,
				String.Format(OperationPaths.InfoShareSTSWebConfig.STSServiceBehaviorsTrustedUser, menuItem.Thumbprint), (ActAsTrustedIssuerThumbprintItem)menuItem, false));

			_invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, OperationPaths.InfoShareSTSWebConfig.Path,
				OperationPaths.InfoShareSTSWebConfig.TrustedIssuerBehaviorExtensions));
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
