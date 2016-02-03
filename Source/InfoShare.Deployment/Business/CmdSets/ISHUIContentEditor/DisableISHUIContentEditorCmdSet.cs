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

        private readonly string[] _commentPatterns = { CommentPatterns.XopusAddCheckOut };
        private readonly string[] _commentPatternsExt = { CommentPatterns.XopusAddCheckOut, CommentPatterns.XopusAddUndoCheckOut };
        private readonly string[] _uncommentPatterns = { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn };

        public DisableISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation");
            
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.FolderButtonbar), _commentPatternsExt));
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.InboxButtonBar), _commentPatterns));
            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.InboxButtonBar), _uncommentPatterns));
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LanguageDocumentButtonBar), _commentPatterns));
            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LanguageDocumentButtonBar), _uncommentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
