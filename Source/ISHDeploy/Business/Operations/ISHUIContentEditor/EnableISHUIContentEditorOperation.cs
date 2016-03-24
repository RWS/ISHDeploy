using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIContentEditor
{
    /// <summary>
    /// Enables Content Editor for Content Manager deployment
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHUIContentEditorOperation : IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHUIContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="paths">Reference for all files paths.</param>
        public EnableISHUIContentEditorOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Enabling InfoShare Content Editor");
            
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, paths.FolderButtonbar, new [] { CommentPatterns.XopusAddCheckOut, CommentPatterns.XopusAddUndoCheckOut }));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, paths.InboxButtonBar, CommentPatterns.XopusAddCheckOut));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, paths.InboxButtonBar, new[] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, paths.LanguageDocumentButtonBar, CommentPatterns.XopusAddCheckOut));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, paths.LanguageDocumentButtonBar, new[] { CommentPatterns.XopusRemoveCheckoutDownload, CommentPatterns.XopusRemoveCheckIn }));
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
