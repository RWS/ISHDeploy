using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHUIQualityAssistant
{
    /// <summary>
    /// Enables quality assistant plugin for Content Manager deployment.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHUIQualityAssistantOperation : OperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableISHUIQualityAssistantOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public EnableISHUIQualityAssistantOperation(ILogger logger)
        {
			_invoker = new ActionInvoker(logger, "Enabling of InfoShare Enrich integration for Content Editor");

			_invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, XopusBluelionConfigXml.Path, XopusBluelionConfigXml.EnrichIntegrationBluelionConfig));
			_invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, XopusConfigXml.Path, XopusConfigXml.EnrichIntegration));
            _invoker.AddAction(new InsertBeforeNodeAction(logger, XopusBlueLionPluginWebCconfig.Path, XopusBlueLionPluginWebCconfig.EnrichBluelionWebConfigJsonMimeMapXPath, XopusBlueLionPluginWebCconfig.EnrichBluelionWebConfigRemoveJsonMimeMapXmlString));
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
