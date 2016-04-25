using System;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWS
{
    /// <summary>
    /// Sets WSFederation configuration.
    /// </summary>
    public class SetISHIntegrationSTSWSFederationOperation : OperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationSTSWSFederationOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="endpoint">The URL to issuer endpoint.</param>
        public SetISHIntegrationSTSWSFederationOperation(ILogger logger, Uri endpoint)
        {
            _invoker = new ActionInvoker(logger, "Setting of WSFederation configuration");

            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareAuthorWebConfig.Path, InfoShareAuthorWebConfig.FederationConfigurationXPath, InfoShareAuthorWebConfig.FederationConfigurationAttributeName, endpoint.ToString()));
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
