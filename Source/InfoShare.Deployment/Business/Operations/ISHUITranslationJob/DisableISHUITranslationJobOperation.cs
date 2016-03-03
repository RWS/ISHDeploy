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

            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, paths.EventMonitorMenuBar, CommentPatterns.EventMonitorTranslationJobs));
            _invoker.AddAction(new CommentNodesByInnerPatternAction(logger, paths.TopDocumentButtonbar, CommentPatterns.TopDocumentButtonbarXPath, CommentPatterns.TranslationComment));
            _invoker.AddAction(new CommentBlockAction(logger, paths.TreeHtm, CommentPatterns.TranslationJobHack));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
