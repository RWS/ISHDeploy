using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHExternalPreview
{
    public class EnableISHExternalPreviewOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

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
                    externalId ?? "ServiceUser"));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
