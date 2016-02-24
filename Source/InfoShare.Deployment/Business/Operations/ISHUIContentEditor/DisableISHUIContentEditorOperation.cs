using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIContentEditor
{
    public class DisableISHUIContentEditorOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

        public DisableISHUIContentEditorOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Disabling InfoShare Content Editor");
            
            _invoker.AddAction(new XmlBlockCommentAction(logger, paths.FolderButtonbar, new [] { CommentPatterns.XopusAddCheckOut, CommentPatterns.XopusAddUndoCheckOut }));
            _invoker.AddAction(new XmlBlockCommentAction(logger, paths.InboxButtonBar, CommentPatterns.XopusAddCheckOut));
            _invoker.AddAction(new XmlBlockUncommentAction(logger, paths.InboxButtonBar, new [] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
            _invoker.AddAction(new XmlBlockCommentAction(logger, paths.LanguageDocumentButtonBar, CommentPatterns.XopusAddCheckOut));
            _invoker.AddAction(new XmlBlockUncommentAction(logger, paths.LanguageDocumentButtonBar, new[] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
