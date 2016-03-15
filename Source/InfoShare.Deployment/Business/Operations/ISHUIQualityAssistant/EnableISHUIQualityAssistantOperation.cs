using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIQualityAssistant
{
    /// <summary>
    /// Enables quality assistant plugin for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHUIQualityAssistantOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHUIQualityAssistantOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="paths">Reference for all files paths.</param>
        public EnableISHUIQualityAssistantOperation(ILogger logger, ISHPaths paths)
        {
			_invoker = new ActionInvoker(logger, "Enabling InfoShare Enrich integration for Content Editor");

			_invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, paths.EnrichConfig, CommentPatterns.EnrichIntegrationBluelionConfig));
			_invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, paths.XopusConfig, CommentPatterns.EnrichIntegration));
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
