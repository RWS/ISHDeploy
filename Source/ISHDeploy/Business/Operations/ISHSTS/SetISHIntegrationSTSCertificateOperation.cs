using System;
using System.ServiceModel.Security;
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
		/// <param name="thumbprint">The certificate thumbprint.</param>
		/// <param name="issuer">The certificate issuer.</param>
		/// <param name="validationMode">The certificate validation mode.</param>
		public SetISHIntegrationSTSCertificateOperation(ILogger logger, string thumbprint, string issuer, X509CertificateValidationMode validationMode)
		{
			_invoker = new ActionInvoker(logger, "Setting of Event Monitor Tab");

			var menuItem = new IssuerThumbprintItem()
			{
				Thumbprint = thumbprint,
				Issuer = issuer
			};

			// Author web Config
			_invoker.AddAction(new SetNodeAction(logger, InfoShareAuthorWebConfig.Path, 
				String.Format(InfoShareAuthorWebConfig.STSIdentityTrustedIssuersXPath, menuItem.Thumbprint), menuItem, false));

			_invoker.AddAction(new SetAttributeValueAction(logger, InfoShareAuthorWebConfig.Path,
				InfoShareAuthorWebConfig.CertificateValidationModeXPath, validationMode.ToString()));

			// WS web Config
			_invoker.AddAction(new SetNodeAction(logger, InfoShareWSWebConfig.Path, 
				String.Format(InfoShareAuthorWebConfig.STSIdentityTrustedIssuersXPath, menuItem.Thumbprint), menuItem, false));

			_invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path,
				InfoShareWSWebConfig.CertificateValidationModePath, validationMode.ToString()));

			// STS web Config
			var actAsTrustedIssuerThumbprintItem = new ActAsTrustedIssuerThumbprintItem()
			{
				Thumbprint = thumbprint,
				Issuer = issuer
			};

			_invoker.AddAction(new SetNodeAction(logger, InfoShareSTSWebConfig.Path,
				String.Format(InfoShareSTSWebConfig.STSServiceBehaviorsTrustedUser, menuItem.Thumbprint), actAsTrustedIssuerThumbprintItem, false));

			_invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, InfoShareSTSWebConfig.Path,
				InfoShareSTSWebConfig.TrustedIssuerBehaviorExtensions));
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
