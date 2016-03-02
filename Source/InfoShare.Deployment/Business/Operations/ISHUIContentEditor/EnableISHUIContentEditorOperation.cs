using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUIContentEditor
{
    public class EnableISHUIContentEditorOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

        public EnableISHUIContentEditorOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Enabling InfoShare Content Editor");
            
            _invoker.AddAction(new XmlNodesByPrecedingPatternUncommentAction(logger, paths.FolderButtonbar, new [] { CommentPatterns.XopusAddCheckOut, CommentPatterns.XopusAddUndoCheckOut }));
            _invoker.AddAction(new XmlNodesByPrecedingPatternUncommentAction(logger, paths.InboxButtonBar, CommentPatterns.XopusAddCheckOut));
            _invoker.AddAction(new XmlNodesByPrecedingPatternCommentAction(logger, paths.InboxButtonBar, new[] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
            _invoker.AddAction(new XmlNodesByPrecedingPatternUncommentAction(logger, paths.LanguageDocumentButtonBar, CommentPatterns.XopusAddCheckOut));
            _invoker.AddAction(new XmlNodesByPrecedingPatternCommentAction(logger, paths.LanguageDocumentButtonBar, new[] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
