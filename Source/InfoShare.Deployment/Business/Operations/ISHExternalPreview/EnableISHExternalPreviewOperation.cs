using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHExternalPreview
{
    public class EnableISHExternalPreviewOperation : IOperation
    {
        private readonly IActionInvoker _invoker;
        private readonly string[] _uncommentPatterns =
        {
            CommentPatterns.TrisoftExternalPreviewModuleSearchPattern,
            CommentPatterns.SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern,
            CommentPatterns.TrisoftInfoshareWebExternalPreviewModuleSearchPattern
        };

        public EnableISHExternalPreviewOperation(ILogger logger, ISHPaths paths, string externalId)
        {
            _invoker = new ActionInvoker(logger, "InfoShare ExternalPreview activation");
            
            _invoker.AddAction(new XmlNodeUncommentAction(logger, paths.AuthorAspWebConfig, _uncommentPatterns));

            _invoker.AddAction(
                new XmlSetAttributeValueAction(
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
