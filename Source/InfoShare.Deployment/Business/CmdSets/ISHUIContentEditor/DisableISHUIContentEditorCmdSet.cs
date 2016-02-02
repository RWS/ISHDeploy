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

            _invoker.AddCommand(
                new XmlCommentCommand(
                    logger, 
                    Path.Combine(ishProject.AuthorFolderPath, 
                    ISHPaths.FolderButtonbar), 
                    new List<string>
                        {
                            CommentPatterns.XopusAddCheckOut,
                            CommentPatterns.XopusAddUndoCheckOut,
                        }));

            _invoker.AddCommand(
                new XmlCommentCommand(
                    logger,
                    Path.Combine(ishProject.AuthorFolderPath,
                    ISHPaths.InboxButtonBar),
                    CommentPatterns.XopusRemoveCheckoutDownload));

            _invoker.AddCommand(
                new XmlCommentCommand(
                    logger,
                    Path.Combine(ishProject.AuthorFolderPath,
                    ISHPaths.LanguageDocumentButtonBar),
                    CommentPatterns.XopusRemoveCheckoutDownload));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
