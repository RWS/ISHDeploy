using System.Linq;
using System.ServiceModel.Security;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHAPIWCFService
{
    /// <summary>
    /// Sets Thumbprint values to WCF Service configuration.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHAPIWCFServiceCertificateOperation : BasePathsOperation, IOperation
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
        /// <param name="validationMode">The certificate validation mode.</param>
        public SetISHAPIWCFServiceCertificateOperation(ILogger logger, Models.ISHDeployment ishDeployment, string thumbprint, X509CertificateValidationMode validationMode) :
            base(logger, ishDeployment)
		{
			_invoker = new ActionInvoker(logger, "Setting of Thumbprint and issuers values to configuration");

            var normalizedThumbprint = new string(thumbprint.ToCharArray().Where(char.IsLetterOrDigit).ToArray());

		    if (normalizedThumbprint.Length != thumbprint.Length)
		    {
                logger.WriteWarning($"The thumbprint '{thumbprint}' has been normalized to '{normalizedThumbprint}'");
		        thumbprint = normalizedThumbprint;
		    }

            // thumbprint
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareAuthorWebConfig.Path, InfoShareAuthorWebConfig.CertificateReferenceFindValueAttributeXPath, thumbprint));
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareSTSConfig.Path, InfoShareSTSConfig.CertificateThumbprintAttributeXPath, thumbprint));

            // validationMode
            _invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfig.Path, FeedSDLLiveContentConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfig.Path, TranslationOrganizerConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfig.Path, SynchronizeToLiveContentConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.InfoShareWSServiceCertificateValidationModeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.InfoShareWSServiceCertificateValidationModeXPath, validationMode.ToString()));
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
