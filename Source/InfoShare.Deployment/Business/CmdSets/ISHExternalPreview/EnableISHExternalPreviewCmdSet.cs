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
        private readonly string[] _uncommentPatterns = { CommentPatterns.TrisoftExternalPreviewModule, CommentPatterns.SectionTrisoftInfoshareWebExternalPreviewModule, CommentPatterns.IdentityExternalId };

        public EnableISHExternalPreviewCmdSet(ILogger logger, ISHProject ishProject, string externalId)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ExternalPreview activation");
            
            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.WebConfig), _uncommentPatterns));
            _invoker.AddCommand(new XmlSetAttributeCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.WebConfig), "", externalId ?? "ServiceUser"));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
