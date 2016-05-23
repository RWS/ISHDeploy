using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIQualityAssistant
{
    /// <summary>
    /// Disables quality assistant plugin for Content Manager development.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHUIQualityAssistantOperation : OperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHUIQualityAssistantOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DisableISHUIQualityAssistantOperation(ILogger logger)
        {
            _invoker = new ActionInvoker(logger, "Disabling of InfoShare Enrich integration for Content Editor");

			_invoker.AddAction(new CommentNodeByXPathAction(logger, XopusBluelionConfigXml.Path, XopusBluelionConfigXml.EnrichIntegrationBluelionConfigXPath));
			_invoker.AddAction(new CommentNodeByXPathAction(logger, XopusConfigXml.Path, XopusConfigXml.EnrichIntegrationXPath));
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
