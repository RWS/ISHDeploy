using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWSTrust
{
    /// <summary>
    /// Sets WSTrust configuration.
    /// </summary>
    public class SetISHIntegrationSTSWSTrustOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationSTSWSTrustOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
		/// <param name="paths">Reference for all files paths.</param>
        /// <param name="endpoint">The URL to issuer WSTrust endpoint.</param>
        /// <param name="mexEndpoint">The URL to issuer WSTrust mexEndpoint.</param>
        /// <param name="bindingType">The STS issuer authentication type.</param>
        public SetISHIntegrationSTSWSTrustOperation(ILogger logger, ISHPaths paths, Uri endpoint, Uri mexEndpoint, BindingTypes bindingType)
        {
            _invoker = new ActionInvoker(logger, "Setting of WSTrust configuration");

            _invoker.AddAction(new SetElementValueAction(logger, paths.InfoShareWSConnectionConfig, CommentPatterns.WSTrustEndpointUrlXPath, endpoint.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, paths.InfoShareWSConnectionConfig, CommentPatterns.WSTrustEndpointAuthenticationTypeXPath, bindingType.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, paths.InfoShareWSWebConfig, CommentPatterns.WSTrustMexEndpointBindingHttpUrlXPath, CommentPatterns.WSTrustMexEndpointBindingIssuerMetadataAttributeName, mexEndpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, paths.InfoShareWSWebConfig, CommentPatterns.WSTrustMexEndpointBindingHttpsUrlXPath, CommentPatterns.WSTrustMexEndpointBindingIssuerMetadataAttributeName, mexEndpoint.ToString()));
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
