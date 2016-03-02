using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIQualityAssistant
{
    public class EnableISHUIQualityAssistantOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

        public EnableISHUIQualityAssistantOperation(ILogger logger, ISHPaths paths)
        {
			_invoker = new ActionInvoker(logger, "Enabling InfoShare Enrich integration for Content Editor");

			_invoker.AddAction(new XmlNodesByInnerPatternUncommentAction(logger, paths.EnrichConfig, CommentPatterns.EnrichIntegrationBluelionConfig));
			_invoker.AddAction(new XmlNodesByInnerPatternUncommentAction(logger, paths.XopusConfig, CommentPatterns.EnrichIntegration));
		}

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
