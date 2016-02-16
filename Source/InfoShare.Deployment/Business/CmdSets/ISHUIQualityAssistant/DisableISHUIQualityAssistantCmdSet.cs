using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.CmdSets.ISHUIQualityAssistant
{
    public class DisableISHUIQualityAssistantCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public DisableISHUIQualityAssistantCmdSet(ILogger logger, ISHPaths paths)
        {
            _invoker = new CommandInvoker(logger, "InfoShare Enrich integration for Create");

			_invoker.AddCommand(new XmlNodeCommentCommand(logger, paths.EnrichConfig, CommentPatterns.EnrichIntegrationBluelionConfigXPath));
			_invoker.AddCommand(new XmlNodeCommentCommand(logger, paths.XopusConfig, CommentPatterns.EnrichIntegrationXPath));
		}

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
