using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.TextFile;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHUITranslationJob
{
    /// <summary>
    /// Enables translation job functionality for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHUITranslationJobOperation : IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHUITranslationJobOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="paths">Reference for all files paths.</param>
        public EnableISHUITranslationJobOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Enabling InfoShare translation job");
            
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, paths.EventMonitorMenuBar, CommentPatterns.EventMonitorTranslationJobs));
            _invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, paths.TopDocumentButtonbar, CommentPatterns.TranslationJobAttribute, true));
            _invoker.AddAction(new UncommentBlockAction(logger, paths.TreeHtm, CommentPatterns.TranslationJobHack));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
