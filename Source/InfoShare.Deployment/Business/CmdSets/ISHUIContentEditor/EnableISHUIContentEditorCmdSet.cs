using System.Collections.Generic;
using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor
{
    public class EnableISHUIContentEditorCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public EnableISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation");

            var uncommentPatterns = new[]
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut,
            };

            var commentPatterns = new[]
            {
                CommentPatterns.XopusRemoveCheckoutDownload,
                CommentPatterns.XopusRemoveCheckIn
            };

            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.FolderButtonbar), null, uncommentPatterns));
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.InboxButtonBar), commentPatterns, uncommentPatterns));
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LanguageDocumentButtonBar), commentPatterns, uncommentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
