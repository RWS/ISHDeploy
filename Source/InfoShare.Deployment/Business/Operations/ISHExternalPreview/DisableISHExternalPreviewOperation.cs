using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Business.Operations.ISHExternalPreview
{
    public class DisableISHExternalPreviewOperation : IOperation
    {
        private readonly IActionInvoker _invoker;

        public DisableISHExternalPreviewOperation(ILogger logger, ISHPaths paths)
        {
            _invoker = new ActionInvoker(logger, "InfoShare ExternalPreview deactivation");

            _invoker.AddAction(
                new XmlSetAttributeValueAction(
                    logger,
                    paths.AuthorAspWebConfig, 
                    CommentPatterns.TrisoftInfoshareWebExternalXPath, 
                    CommentPatterns.TrisoftInfoshareWebExternalAttributeName, 
                    "THE_FISHEXTERNALID_TO_USE"));

            _invoker.AddAction(
                new XmlNodeCommentAction(
                    logger,
                    paths.AuthorAspWebConfig,
                    new [] {
                        CommentPatterns.TrisoftExternalPreviewModuleXPath,
                        CommentPatterns.SectionTrisoftInfoshareWebExternalPreviewModuleXPath,
                        CommentPatterns.TrisoftInfoshareWebExternalPreviewModuleXPath
                    }));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
