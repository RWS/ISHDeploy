using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations.ISHUIEventMonitorTab;

namespace ISHDeploy.Cmdlets.ISHUIEventMonitorTab
{
	/// <summary>
	/// <para type="synopsis">Removes tab from EventMonitorTab.</para>
	/// <para type="description">The Removes-ISHUIEventMonitorTab cmdlet removes Tabs definitions from Content Manager deployment.</para>
	/// <para type="link">Set-ISHUIEventMonitorTab</para>
	/// <para type="link">Move-ISHUIEventMonitorTab</para>
	/// </summary>
	/// <example>
	/// <code>PS C:\>Remove-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "Translation"</code>
	/// <para>Removes definition of the tab with label "Translation".</para>
	/// <para>This command removes XML definitions from EventMonitor.
	/// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
	/// </example>
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
