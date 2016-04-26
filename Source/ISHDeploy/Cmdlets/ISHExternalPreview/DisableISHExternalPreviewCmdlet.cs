using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHExternalPreview;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Validators;

namespace ISHDeploy.Cmdlets.ISHExternalPreview
{
    /// <summary>
    /// <para type="synopsis">Disables external preview for Content Manager deployment.</para>
    /// <para type="description">The Disable-ISHExternalPreview cmdlet disables external preview for Content Manager deployment.</para>
    /// <para type="link">Enable-ISHExternalPreview</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Disable-ISHExternalPreview -ISHDeployment $deployment</code>
    /// <para>This command disables the external preview.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Disable, "ISHExternalPreview")]
    public sealed class DisableISHExternalPreviewCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        [ValidateDeploymentVersion]
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
            OperationPaths.Initialize(ISHDeployment);
            var operation = new DisableISHExternalPreviewOperation(Logger);

            operation.Run();
        }
    }
}
