using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHExternalPreview;

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
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHExternalPreviewOperation(Logger, ISHDeployment);

            operation.Run();
        }
    }
}
