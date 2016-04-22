using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHExternalPreview
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
        /// <param name="externalId">The external user identifier.</param>
        public EnableISHExternalPreviewOperation(ILogger logger, string externalId)
        {
            _invoker = new ActionInvoker(logger, "Enabling of InfoShare external preview");
            
            _invoker.AddAction(
                new UncommentNodesByInnerPatternAction(
                    logger,
					OperationPaths.InfoShareAuthorWebConfig.Path,
                    new [] {
						OperationPaths.InfoShareAuthorWebConfig.TrisoftExternalPreviewModuleSearchPattern,
						OperationPaths.InfoShareAuthorWebConfig.SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern,
						OperationPaths.InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalPreviewModuleSearchPattern
                    }));

            _invoker.AddAction(
                new SetAttributeValueAction(
                    logger,
					OperationPaths.InfoShareAuthorWebConfig.Path,
					OperationPaths.InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalXPath,
					OperationPaths.InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalAttributeName, 
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
