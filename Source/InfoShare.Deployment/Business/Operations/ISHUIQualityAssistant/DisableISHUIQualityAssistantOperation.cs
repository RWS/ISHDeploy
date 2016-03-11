using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIQualityAssistant
{
    /// <summary>
    /// Disables quality assistant plugin for Content Manager development.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHUIQualityAssistantOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHUIQualityAssistantOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="paths">Reference for all files paths.</param>
        public DisableISHUIQualityAssistantOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Disabling InfoShare Enrich integration for Content Editor");

			_invoker.AddAction(new CommentNodeByXPathAction(logger, paths.EnrichConfig, CommentPatterns.EnrichIntegrationBluelionConfigXPath));
			_invoker.AddAction(new CommentNodeByXPathAction(logger, paths.XopusConfig, CommentPatterns.EnrichIntegrationXPath));
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
