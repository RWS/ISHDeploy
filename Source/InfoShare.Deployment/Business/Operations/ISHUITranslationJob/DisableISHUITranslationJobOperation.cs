using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.TextFile;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUITranslationJob
{
    public class DisableISHUITranslationJobOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

        public DisableISHUITranslationJobOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Disabling InfoShare translation job");

            _invoker.AddAction(new XmlNodeUncommentAction(logger, paths.EventMonitorMenuBar, CommentPatterns.TranslationJobsXPath));
            _invoker.AddAction(new XmlNodeInternalMarkCommentAction(logger, paths.TopDocumentButtonbar, CommentPatterns.TranslationInternalCommentXPath, CommentPatterns.TranslationExternalCommentXPath, CommentPatterns.TranslationComment));
            _invoker.AddAction(new TextBlockUncommentAction(logger, paths.TreeHtm, CommentPatterns.TranslationJobHack));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
