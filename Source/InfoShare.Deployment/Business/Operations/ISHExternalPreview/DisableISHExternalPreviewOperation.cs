using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHExternalPreview
{
    /// <summary>
    /// Disables external preview for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHExternalPreviewOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHExternalPreviewOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="paths">Reference for all files paths.</param>
        public DisableISHExternalPreviewOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "Disabling InfoShare external preview");

            _invoker.AddAction(new SetAttributeValueAction(
                    logger,
                    paths.AuthorAspWebConfig, 
                    CommentPatterns.TrisoftInfoshareWebExternalXPath, 
                    CommentPatterns.TrisoftInfoshareWebExternalAttributeName, 
                    "THE_FISHEXTERNALID_TO_USE"));

            _invoker.AddAction(new CommentNodeByXPathAction(
                    logger,
                    paths.AuthorAspWebConfig,
                    new [] {
                        CommentPatterns.TrisoftExternalPreviewModuleXPath,
                        CommentPatterns.SectionTrisoftInfoshareWebExternalPreviewModuleXPath,
                        CommentPatterns.TrisoftInfoshareWebExternalPreviewModuleXPath
                    }));
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
