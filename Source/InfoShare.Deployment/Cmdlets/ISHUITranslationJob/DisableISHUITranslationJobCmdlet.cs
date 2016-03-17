using System.Management.Automation;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Business.Operations.ISHUITranslationJob;

namespace InfoShare.Deployment.Cmdlets.ISHUITranslationJob
{
    /// <summary>
    /// <para type="synopsis">Disables translation job for Content Manager deployment.</para>
    /// <para type="description">The Disable-ISHUITranslationJob cmdlet disables translation job for Content Manager deployment.</para>
    /// <para type="link">Enable-ISHUITranslationJob</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Disable-ISHUITranslationJob -ISHDeployment $deployment</code>
    /// <para>This command disables translation job.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Disable, "ISHUITranslationJob")]
    public class DisableISHUITranslationJobCmdlet : BaseHistoryEntryCmdlet
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
            var operation = new DisableISHUITranslationJobOperation(Logger, IshPaths);

            operation.Run();
        }
    }
}
