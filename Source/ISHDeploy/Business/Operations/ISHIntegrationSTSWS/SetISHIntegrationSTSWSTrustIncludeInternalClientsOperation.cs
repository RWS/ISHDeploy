using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWS
{
    /// <summary>
    /// Sets WSTrust configuration including internal clients.
    /// </summary>
    public class SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation : OperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="endpoint">The URL to issuer WSTrust endpoint.</param>
        /// <param name="mexEndpoint">The URL to issuer WSTrust mexEndpoint.</param>
        /// <param name="bindingType">The STS issuer authentication type.</param>
        /// <param name="actorUsername">The STS user.</param>
        /// <param name="actorPassword">The password of STS user.</param>
        public SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation(ILogger logger, Uri endpoint, Uri mexEndpoint, BindingTypes bindingType, string actorUsername, string actorPassword)
        {
            _invoker = new ActionInvoker(logger, "Setting of WSTrust configuration including internal clients");

            // endpoint
            _invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfig.Path, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfig.Path, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfig.Path, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            // mexEndpoint
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpsXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            // bindingType
            _invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfig.Path, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfig.Path, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfig.Path, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
            // actorUsername and actorPassword
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustActorUserNameXPath, actorUsername));
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustActorPasswordXPath, actorPassword));
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
