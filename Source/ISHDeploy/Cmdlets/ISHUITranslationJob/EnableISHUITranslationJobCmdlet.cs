using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUITranslationJob;

namespace ISHDeploy.Cmdlets.ISHUITranslationJob
{
    /// <summary>
    /// <para type="synopsis">Enable translation job for Content Manager deployment.</para>
    /// <para type="description">The Enable-ISHUITranslationJob cmdlet enables translation job for Content Manager deployment.</para>
    /// <para type="link">Disable-ISHUITranslationJob</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Enable-ISHUITranslationJob -ISHDeployment $deployment</code>
    /// <para>This command enables translation job.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHUITranslationJob")]
    public class EnableISHUITranslationJobCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new EnableISHUITranslationJobOperation(Logger);

            operation.Run();
        }
    }
}
