using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHUIQualityAssistant
{
    public class DisableISHUIQualityAssistantCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        private readonly string[] _uncommentPatterns = { CommentPatterns.EnrichIntegration };

        public DisableISHUIQualityAssistantCmdSet(ILogger logger, ISHProject ishProject)
        {
            _invoker = new CommandInvoker(logger, "InfoShare Enrich integration for Create");

			_invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.EnrichConfig), _uncommentPatterns));
			_invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.XopusConfig), _uncommentPatterns));
		}

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
