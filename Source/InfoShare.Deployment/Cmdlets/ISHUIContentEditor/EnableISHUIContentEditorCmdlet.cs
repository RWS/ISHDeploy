using System.Management.Automation;
using InfoShare.Deployment.Business.Operations.ISHUIContentEditor;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHUIContentEditor
{
    /// <summary>
    /// <para type="synopsis">Enables Content Editor for Content Manager deployment.</para>
    /// <para type="description">The Enable-ISHUIContentEditor cmdlet enables Content Editor for Content Manager deployment.</para>
    /// <para type="link">Disable-ISHUIContentEditor</para>
    /// </summary>
    /// <example>
    /// <para>Enable Content Editor:</para>
    /// <code>Enable-ISHUIContentEditor -ISHDeployment $deployment</code>
    /// <para>Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHUIContentEditor")]
    public sealed class EnableISHUIContentEditorCmdlet : BaseHistoryEntryCmdlet
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
            var operation = new EnableISHUIContentEditorOperation(Logger, IshPaths);

            operation.Run();
        }
    }
}