using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIContentEditor
{
    /// <summary>
    /// Enables Content Editor for Content Manager deployment
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHUIContentEditorOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHUIContentEditorOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public EnableISHUIContentEditorOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Enabling of InfoShare Content Editor");
            
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, FolderButtonBarXml.Path, new [] { FolderButtonBarXml.XopusAddCheckOut, FolderButtonBarXml.XopusAddUndoCheckOut }));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, InboxButtonBarXml.Path, InboxButtonBarXml.XopusAddCheckOut));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, InboxButtonBarXml.Path, new[] { InboxButtonBarXml.XopusRemoveCheckoutDownload, InboxButtonBarXml.XopusRemoveCheckIn }));
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, LanguageDocumentButtonbarXml.Path, LanguageDocumentButtonbarXml.XopusAddCheckOut));
            _invoker.AddAction(new CommentNodesByPrecedingPatternAction(logger, LanguageDocumentButtonbarXml.Path, new[] { LanguageDocumentButtonbarXml.XopusRemoveCheckoutDownload, LanguageDocumentButtonbarXml.XopusRemoveCheckIn }));
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
