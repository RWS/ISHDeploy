using System;
using System.Linq;
using System.ServiceModel.Security;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;
using ISHDeploy.Models.ISHXmlNodes;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Sets Thumbprint and issuers values to configuration.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIntegrationSTSCertificateOperation : BasePathsOperation, IOperation
	{
		/// <summary>
		/// The actions invoker
		/// </summary>
		private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <param name="issuer">The certificate issuer.</param>
        /// <param name="validationMode">The certificate validation mode.</param>
        public SetISHIntegrationSTSCertificateOperation(ILogger logger, Models.ISHDeployment ishDeployment, string thumbprint, string issuer, X509CertificateValidationMode validationMode) : 
            base(logger, ishDeployment)
		{
			_invoker = new ActionInvoker(logger, "Setting of Thumbprint and issuers values to configuration");

            var normalizedThumbprint = new string(thumbprint.ToCharArray().Where(char.IsLetterOrDigit).ToArray());

		    if (normalizedThumbprint.Length != thumbprint.Length)
		    {
                logger.WriteWarning($"The thumbprint '{thumbprint}' has been normalized to '{normalizedThumbprint}'");
		        thumbprint = normalizedThumbprint;
		    }

		    var menuItem = new IssuerThumbprintItem()
			{
				Thumbprint = thumbprint,
				Issuer = issuer
			};

			// Author web Config
			_invoker.AddAction(new SetNodeAction(logger, InfoShareAuthorWebConfig.Path, 
				String.Format(InfoShareAuthorWebConfig.IdentityTrustedIssuersByNameXPath, menuItem.Issuer), menuItem));

			_invoker.AddAction(new SetAttributeValueAction(logger, InfoShareAuthorWebConfig.Path,
				InfoShareAuthorWebConfig.CertificateValidationModeXPath, validationMode.ToString()));

			// WS web Config
			_invoker.AddAction(new SetNodeAction(logger, InfoShareWSWebConfig.Path, 
				String.Format(InfoShareWSWebConfig.IdentityTrustedIssuersByNameXPath, menuItem.Issuer), menuItem));

			_invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path,
				InfoShareWSWebConfig.CertificateValidationModeXPath, validationMode.ToString()));

			// STS web Config
			var actAsTrustedIssuerThumbprintItem = new ActAsTrustedIssuerThumbprintItem()
			{
				Thumbprint = thumbprint,
				Issuer = issuer
			};

			_invoker.AddAction(new SetNodeAction(logger, InfoShareSTSWebConfig.Path,
				String.Format(InfoShareSTSWebConfig.ServiceBehaviorsTrustedUserByNameXPath, menuItem.Issuer), actAsTrustedIssuerThumbprintItem));

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
