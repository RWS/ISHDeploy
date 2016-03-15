using System.Management.Automation;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Business.Operations.ISHUIEventMonitorTab;

namespace InfoShare.Deployment.Cmdlets.ISHUIEventMonitorTab
{
	/// <summary>
	/// 
	/// </summary>
    [Cmdlet(VerbsCommon.Remove, "ISHUIEventMonitorTab")]
    public class RemoveISHUIEventMonitorTabCmdlet : BaseHistoryEntryCmdlet
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
		/// <para type="description">Label of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Label of menu item")]
		[ValidateNotNullOrEmpty]
		public string Label { get; set; }

		/// <summary>
		/// Returns instance of the <see cref="ISHPaths"/>
		/// </summary>
		protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new RemoveISHUIEventMonitorTabOperation(Logger, IshPaths, Label);

			operation.Run();
        }
    }
}
