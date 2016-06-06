using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIQualityAssistant;

namespace ISHDeploy.Cmdlets.ISHUIQualityAssistant
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
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHUIQualityAssistantOperation(Logger, ISHDeployment);
            
            operation.Run();
        }
    }
}
