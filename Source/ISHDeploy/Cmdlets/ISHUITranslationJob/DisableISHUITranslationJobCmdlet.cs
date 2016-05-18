using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUITranslationJob;

namespace ISHDeploy.Cmdlets.ISHUITranslationJob
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
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new DisableISHUITranslationJobOperation(Logger);

            operation.Run();
        }
    }
}
