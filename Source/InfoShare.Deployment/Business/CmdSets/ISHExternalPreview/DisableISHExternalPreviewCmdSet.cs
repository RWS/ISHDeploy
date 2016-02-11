using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHExternalPreview
{
    public class DisableISHExternalPreviewCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        private readonly string[] _commentPatterns = { CommentPatterns.TrisoftExternalPreviewModule, CommentPatterns.SectionTrisoftInfoshareWebExternalPreviewModule, CommentPatterns.IdentityExternalId };

        public DisableISHExternalPreviewCmdSet(ILogger logger, ISHProject ishProject)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ExternalPreview deactivation");

            _invoker.AddCommand(new XmlSetAttributeCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.WebConfig), "", "THE_FISHEXTERNALID_TO_USE"));
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.WebConfig), _commentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
