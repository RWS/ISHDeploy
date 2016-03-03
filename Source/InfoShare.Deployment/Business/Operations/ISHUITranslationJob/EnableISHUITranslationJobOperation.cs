using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.TextFile;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUITranslationJob
{
    public class EnableISHUITranslationJobOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

        public EnableISHUITranslationJobOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Enabling InfoShare translation job");
            
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, paths.EventMonitorMenuBar, CommentPatterns.EventMonitorTranslationJobs));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, paths.TopDocumentButtonbar, CommentPatterns.TranslationComment));
            _invoker.AddAction(new UncommentBlockAction(logger, paths.TreeHtm, CommentPatterns.TranslationJobHack));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
