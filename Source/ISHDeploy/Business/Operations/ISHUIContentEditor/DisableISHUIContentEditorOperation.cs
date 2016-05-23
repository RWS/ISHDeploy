using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIContentEditor
{
    /// <summary>
    /// Disables Content Editor for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHUIContentEditorOperation : OperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHUIContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DisableISHUIContentEditorOperation(ILogger logger)
        {
            _invoker = new ActionInvoker(logger, "Disabling of InfoShare Content Editor");
            
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, FolderButtonBarXml.Path, new [] { FolderButtonBarXml.XopusAddCheckOut, FolderButtonBarXml.XopusAddUndoCheckOut }));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, InboxButtonBarXml.Path, InboxButtonBarXml.XopusAddCheckOut));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, InboxButtonBarXml.Path, new [] { InboxButtonBarXml.XopusRemoveCheckoutDownload, InboxButtonBarXml.XopusRemoveCheckIn }));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, LanguageDocumentButtonbarXml.Path, LanguageDocumentButtonbarXml.XopusAddCheckOut));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, LanguageDocumentButtonbarXml.Path, new[] { LanguageDocumentButtonbarXml.XopusRemoveCheckoutDownload, LanguageDocumentButtonbarXml.XopusRemoveCheckIn }));
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
