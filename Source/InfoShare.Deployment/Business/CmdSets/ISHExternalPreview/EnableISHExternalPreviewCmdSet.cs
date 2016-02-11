using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHExternalPreview
{
    public class EnableISHExternalPreviewCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;
        private readonly string[] _uncommentPatterns = { CommentPatterns.TrisoftExternalPreviewModuleSearchPattern, CommentPatterns.SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern, CommentPatterns.TrisoftInfoshareWebExternalPreviewModuleSearchPattern };

        public EnableISHExternalPreviewCmdSet(ILogger logger, ISHProject ishProject, string externalId)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ExternalPreview activation");
            
            _invoker.AddCommand(new XmlNodeUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.WebConfig), _uncommentPatterns));
            _invoker.AddCommand(new XmlSetAttributeValueCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.WebConfig), CommentPatterns.TrisoftInfoshareWebExternalXPath, CommentPatterns.TrisoftInfoshareWebExternalAttributeName, externalId ?? "ServiceUser"));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
