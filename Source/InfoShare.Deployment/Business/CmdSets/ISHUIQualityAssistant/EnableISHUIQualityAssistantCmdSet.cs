using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.CmdSets.ISHUIQualityAssistant
{
    public class EnableISHUIQualityAssistantCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public EnableISHUIQualityAssistantCmdSet(ILogger logger, ISHPaths paths)
        {
			_invoker = new CommandInvoker(logger, "InfoShare Enrich integration for Create");

			_invoker.AddCommand(new XmlNodeUncommentCommand(logger, paths.EnrichConfig, CommentPatterns.EnrichIntegrationBluelionConfig));
			_invoker.AddCommand(new XmlNodeUncommentCommand(logger, paths.XopusConfig, CommentPatterns.EnrichIntegration));
		}

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
