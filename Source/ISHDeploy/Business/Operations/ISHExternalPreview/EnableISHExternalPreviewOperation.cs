using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHExternalPreview
{
    /// <summary>
    /// Enables external preview for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHExternalPreviewOperation : OperationPaths, IOperation
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
                    InfoShareAuthorWebConfig.Path,
                    new [] {
                        InfoShareAuthorWebConfig.TrisoftExternalPreviewModuleSearchPattern,
                        InfoShareAuthorWebConfig.SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern,
                        InfoShareAuthorWebConfig.TrisoftInfoshareWebExternalPreviewModuleSearchPattern
                    }));

            _invoker.AddAction(
                new SetAttributeValueAction(
                    logger,
                    InfoShareAuthorWebConfig.Path,
                    InfoShareAuthorWebConfig.ExternalPreviewModuleXPath,
                    InfoShareAuthorWebConfig.ExternalPreviewModuleAttributeName, 
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
