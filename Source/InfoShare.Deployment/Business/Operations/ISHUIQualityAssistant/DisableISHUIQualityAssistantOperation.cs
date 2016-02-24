using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIQualityAssistant
{
    public class DisableISHUIQualityAssistantOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

        public DisableISHUIQualityAssistantOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Disabling InfoShare Enrich integration for Content Editor");

			_invoker.AddAction(new XmlNodeCommentAction(logger, paths.EnrichConfig, CommentPatterns.EnrichIntegrationBluelionConfigXPath));
			_invoker.AddAction(new XmlNodeCommentAction(logger, paths.XopusConfig, CommentPatterns.EnrichIntegrationXPath));
		}

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
