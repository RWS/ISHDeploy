using System.Collections.Generic;
using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor
{
    public class DisableISHUIContentEditorCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public DisableISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation", enableBackup);

            var commentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut,
            };

            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.FolderButtonbar), commentPatterns));

            //TODO: ask what to do with that? commentPatterns.Add(CommentPatterns.XopusRemoveCheckoutDownload);
            //TODO: ask what to do with that? commentPatterns.Add(CommentPatterns.XopusRemoveCheckIn);
            //TODO: ask what to do with that? commentPatterns.Add(CommentPatterns.XopusRemoveUndoCheckOut);

            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.InboxButtonBar), commentPatterns));
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LanguageDocumentButtonBar), commentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
