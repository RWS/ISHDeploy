using System.Management.Automation;
using InfoShare.Deployment.Business.Operations.ISHUIQualityAssistant;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHUIQualityAssistant
{
    /// <summary>
    /// <para type="synopsis">Disables Quality Assistant for Content Manager deployment.</para>
    /// <para type="description">The Disable-ISHUIQualityAssistant cmdlet disables Quality Assistant for Content Manager deployment.</para>
    /// <para type="link">Enable-ISHUIQualityAssistant</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Disable-ISHUIQualityAssistant -ISHDeployment $deployment</code>
    /// <para>This command disables Quality Assistant.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIQualityAssistant")]
    public sealed class DisableISHUIQualityAssistantCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// Cashed value for <see cref="IshPaths"/> property
        /// </summary>
        private ISHPaths _ishPaths;

        /// <summary>
        /// Returns instance of the <see cref="ISHPaths"/>
        /// </summary>
        protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHUIQualityAssistantOperation(Logger, IshPaths);
            
            operation.Run();
        }
    }
}
