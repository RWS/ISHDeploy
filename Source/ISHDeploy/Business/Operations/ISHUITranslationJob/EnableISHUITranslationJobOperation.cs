using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.TextFile;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUITranslationJob
{
    /// <summary>
    /// Enables translation job functionality for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHUITranslationJobOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHUITranslationJobOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public EnableISHUITranslationJobOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Enabling of InfoShare translation job");
            
            _invoker.AddAction(new UncommentNodesByPrecedingPatternAction(logger, EventMonitorMenuBarXml.Path, EventMonitorMenuBarXml.EventMonitorTranslationJobs));
            _invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, TopDocumentButtonBarXml.Path, TopDocumentButtonBarXml.TranslationJobAttribute, true));
            _invoker.AddAction(new UncommentBlockAction(logger, AuthorASPTreeHtm.Path, AuthorASPTreeHtm.TranslationJobHack));
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
