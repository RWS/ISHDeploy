using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIContentEditor;

namespace ISHDeploy.Cmdlets.ISHUIContentEditor
{
    /// <summary>
    /// <para type="synopsis">Disables Content Editor for Content Manager deployment.</para>
    /// <para type="description">The Disable-ISHUIContentEditor cmdlet disables Content Editor for Content Manager deployment.</para>
    /// <para type="link">Enable-ISHUIContentEditor</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Disable-ISHUIContentEditor -ISHDeployment $deployment</code>
    /// <para>This command disables Content Editor.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIContentEditor")]
    public sealed class DisableISHUIContentEditorCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHUIContentEditorOperation(Logger);

            operation.Run();
        }
    }
}
