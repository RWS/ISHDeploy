using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHExternalPreview
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
        public DisableISHExternalPreviewOperation(ILogger logger)
        {
            _invoker = new ActionInvoker(logger, "Disabling of InfoShare external preview");

            _invoker.AddAction(new SetAttributeValueAction(
                    logger,
					OperationPaths.InfoShareAuthorWebConfig.Path,
					OperationPaths.InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalXPath,
					OperationPaths.InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalAttributeName, 
                    "THE_FISHEXTERNALID_TO_USE"));

            _invoker.AddAction(new CommentNodeByXPathAction(
                    logger,
					OperationPaths.InfoShareAuthorWebConfig.Path,
                    new [] {
						OperationPaths.InfoShareAuthorWebConfig.TrisoftExternalPreviewModuleXPath,
						OperationPaths.InfoShareAuthorWebConfig.SectionTrisoftInfoshareWebExternalPreviewModuleXPath,
						OperationPaths.InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalPreviewModuleXPath
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
