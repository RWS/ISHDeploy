using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIQualityAssistant;

namespace ISHDeploy.Cmdlets.ISHUIQualityAssistant
{
    /// <summary>
    /// <para type="synopsis">Enables Quality Assistant for Content Manager deployment.</para>
    /// <para type="description">The Enable-ISHUIQualityAssistant cmdlet enables Quality Assistant for Content Manager deployment.</para>
    /// <para type="link">Disable-ISHUIQualityAssistant</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Enable-ISHUIQualityAssistant -ISHDeployment $deployment</code>
    /// <para>This command enables Quality Assistant.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHUIQualityAssistant")]
    public sealed class EnableISHUIQualityAssistantCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new EnableISHUIQualityAssistantOperation(Logger, ISHDeployment);

            operation.Run();
        }
    }
}