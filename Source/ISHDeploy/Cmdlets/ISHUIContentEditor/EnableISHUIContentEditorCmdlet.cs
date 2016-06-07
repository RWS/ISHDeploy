using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIContentEditor;

namespace ISHDeploy.Cmdlets.ISHUIContentEditor
{
    /// <summary>
    /// <para type="synopsis">Enables Content Editor for Content Manager deployment.</para>
    /// <para type="description">The Enable-ISHUIContentEditor cmdlet enables Content Editor for Content Manager deployment.</para>
    /// <para type="link">Disable-ISHUIContentEditor</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Enable-ISHUIContentEditor -ISHDeployment $deployment</code>
    /// <para>This command enables Content Editor.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHUIContentEditor")]
    public sealed class EnableISHUIContentEditorCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new EnableISHUIContentEditorOperation(Logger, ISHDeployment);

            operation.Run();
        }
    }
}