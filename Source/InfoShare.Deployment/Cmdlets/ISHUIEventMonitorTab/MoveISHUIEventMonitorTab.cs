using System;
using System.Management.Automation;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Business.Operations.ISHUIEventMonitorTab;

namespace InfoShare.Deployment.Cmdlets.ISHUIEventMonitorTab
{
	/// <summary>
	/// Moves tab definitions in EventMonitorTab. Currently only insert First/Last/After functionality is supported.
	/// </summary>
	/// <seealso cref="InfoShare.Deployment.Cmdlets.BaseHistoryEntryCmdlet" />
	[Cmdlet(VerbsCommon.Move, "ISHUIEventMonitorTab")]
    public class MoveISHUIEventMonitorTabCmdlet : BaseHistoryEntryCmdlet
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
		/// <para type="description">Label of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item move position", ParameterSetName = "Last")]
		[ValidateNotNullOrEmpty]
		public SwitchParameter Last  { get; set; }

		/// <summary>
		/// <para type="description">Label of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item move position", ParameterSetName = "First")]
		[ValidateNotNullOrEmpty]
		public SwitchParameter First  { get; set; }

		/// <summary>
		/// <para type="description">Menu item move position.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item move position", ParameterSetName = "After")]
		[ValidateNotNullOrEmpty]
		public string After { get; set; }

		/// <summary>
		/// Returns instance of the <see cref="ISHPaths"/>
		/// </summary>
		protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
	        MoveISHUIEventMonitorTabOperation operation;

			switch (ParameterSetName)
	        {
				case "Last":
					operation = new MoveISHUIEventMonitorTabOperation(Logger, IshPaths, Label, MoveISHUIEventMonitorTabOperation.OperationType.InsertAfter);
					break;
				case "First":
					operation = new MoveISHUIEventMonitorTabOperation(Logger, IshPaths, Label, MoveISHUIEventMonitorTabOperation.OperationType.InsertBefore);
					break;
				case "After":
					operation = new MoveISHUIEventMonitorTabOperation(Logger, IshPaths, Label, MoveISHUIEventMonitorTabOperation.OperationType.InsertAfter, After);
					break;
				default:
					throw new ArgumentException($"Operation type in {nameof(MoveISHUIEventMonitorTabCmdlet)} should be defined.");
	        }

			operation.Run();
        }
    }
}
