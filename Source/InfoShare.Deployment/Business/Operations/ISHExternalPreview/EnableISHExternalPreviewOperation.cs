using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHExternalPreview
{
    /// <summary>
    /// Enables external preview for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHExternalPreviewOperation : IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHExternalPreviewOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="paths">Reference for all files paths.</param>
        /// <param name="externalId">The external user identifier.</param>
        public EnableISHExternalPreviewOperation(ILogger logger, ISHPaths paths, string externalId)
        {
            _invoker = new ActionInvoker(logger, "Enabling InfoShare external preview");
            
            _invoker.AddAction(
                new UncommentNodesByInnerPatternAction(
                    logger,
                    paths.AuthorAspWebConfig,
                    new [] {
                        CommentPatterns.TrisoftExternalPreviewModuleSearchPattern,
                        CommentPatterns.SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern,
                        CommentPatterns.TrisoftInfoshareWebExternalPreviewModuleSearchPattern
                    }));

            _invoker.AddAction(
                new SetAttributeValueAction(
                    logger,
                    paths.AuthorAspWebConfig, 
                    CommentPatterns.TrisoftInfoshareWebExternalXPath, 
                    CommentPatterns.TrisoftInfoshareWebExternalAttributeName, 
                    externalId));
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
