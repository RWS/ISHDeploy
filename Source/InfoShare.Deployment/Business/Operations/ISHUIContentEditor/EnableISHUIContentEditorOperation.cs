using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIContentEditor
{
    public class EnableISHUIContentEditorOperation : IOperation
    {
        private readonly IActionInvoker _invoker;
        private readonly string[] _uncommentPatterns = { CommentPatterns.XopusAddCheckOut };
        private readonly string[] _uncommentPatternsExt = { CommentPatterns.XopusAddCheckOut, CommentPatterns.XopusAddUndoCheckOut };
        private readonly string[] _commentPatterns = { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn };

        public EnableISHUIContentEditorOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "InfoShare ContentEditor activation");
            
            _invoker.AddAction(new XmlBlockUncommentAction(logger, paths.FolderButtonbar, _uncommentPatternsExt));
            _invoker.AddAction(new XmlBlockUncommentAction(logger, paths.InboxButtonBar, _uncommentPatterns));
            _invoker.AddAction(new XmlBlockCommentAction(logger, paths.InboxButtonBar, _commentPatterns));
            _invoker.AddAction(new XmlBlockUncommentAction(logger, paths.LanguageDocumentButtonBar, _uncommentPatterns));
            _invoker.AddAction(new XmlBlockCommentAction(logger, paths.LanguageDocumentButtonBar, _commentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
