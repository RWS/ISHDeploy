using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWS
{
    /// <summary>
    /// Sets WSTrust configuration.
    /// </summary>
    public class SetISHIntegrationSTSWSTrustOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationSTSWSTrustOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="endpoint">The URL to issuer WSTrust endpoint.</param>
        /// <param name="mexEndpoint">The URL to issuer WSTrust mexEndpoint.</param>
        /// <param name="bindingType">The STS issuer authentication type.</param>
        public SetISHIntegrationSTSWSTrustOperation(ILogger logger, Models.ISHDeployment ishDeployment, Uri endpoint, Uri mexEndpoint, BindingType bindingType) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting of WSTrust configuration");

            // endpoint
            _invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            // mexEndpoint
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpsXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            // bindingType
            _invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
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
