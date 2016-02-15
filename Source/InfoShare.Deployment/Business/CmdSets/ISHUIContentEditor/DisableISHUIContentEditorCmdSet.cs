using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor
{
    public class DisableISHUIContentEditorCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        private readonly string[] _commentPatterns = { CommentPatterns.XopusAddCheckOut };
        private readonly string[] _commentPatternsExt = { CommentPatterns.XopusAddCheckOut, CommentPatterns.XopusAddUndoCheckOut };
        private readonly string[] _uncommentPatterns = { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn };

        public DisableISHUIContentEditorCmdSet(ILogger logger, string authorFolderPath)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation");
            
            _invoker.AddCommand(new XmlBlockCommentCommand(logger, Path.Combine(authorFolderPath, ISHPaths.FolderButtonbar), _commentPatternsExt));
            _invoker.AddCommand(new XmlBlockCommentCommand(logger, Path.Combine(authorFolderPath, ISHPaths.InboxButtonBar), _commentPatterns));
            _invoker.AddCommand(new XmlBlockUncommentCommand(logger, Path.Combine(authorFolderPath, ISHPaths.InboxButtonBar), _uncommentPatterns));
            _invoker.AddCommand(new XmlBlockCommentCommand(logger, Path.Combine(authorFolderPath, ISHPaths.LanguageDocumentButtonBar), _commentPatterns));
            _invoker.AddCommand(new XmlBlockUncommentCommand(logger, Path.Combine(authorFolderPath, ISHPaths.LanguageDocumentButtonBar), _uncommentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
