using System;
using System.Linq;
using System.Management.Automation;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Business.Operations.ISHUIEventMonitorTab;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Cmdlets.ISHUIEventMonitorTab
{
	/// <summary>
	/// 
	/// </summary>
    [Cmdlet(VerbsCommon.Set, "ISHUIEventMonitorTab")]
    public class SetISHUIEventMonitorTabCmdlet : BaseHistoryEntryCmdlet
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
		/// <para type="description">Menu item icon representation.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Menu item icon representation")]
		[ValidateNotNullOrEmpty]
		public string Icon { get; set; }

		#region Action parameters

		/// <summary>
		/// <para type="description">Status filter.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Status filter")]
		public string EventTypesFilter { get; set; }

		/// <summary>
		/// <para type="description">Status filter.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Status filter")]
		public string StatusFilter { get; set; }

		/// <summary>
		/// <para type="description">Menu title when selected.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu title when selected")]
		public string SelectedMenuItemTitle { get; set; }

		/// <summary>
		/// <para type="description">Modified since minutes filter value.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Modified since minutes filter value")]
		public int ModifiedSinceMinutesFilter { get; set; }

		/// <summary>
		/// <para type="description">Menu item icon representation.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item icon representation")]
		public string SelectedButtonTitle { get; set; }

		#endregion

		/// <summary>
		/// <para type="description">Action of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Action of menu item")]
		[ValidateNotNullOrEmpty]
		public string UserRole { get; set; }

		/// <summary>
		/// <para type="description">User role description.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "User role description")]
		public string Description { get; set; }
		

		/// <summary>
		/// Returns instance of the <see cref="ISHPaths"/>
		/// </summary>
		protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
			var operation = new SetISHUIEventMonitorTabOperation(Logger, IshPaths, new EventLogMenuItem()
			{
				Label = Label,
				Description = Description,
				Icon = Icon,
				UserRole = UserRole,
				Action = new EventLogMenuItemAction()
				{
					SelectedButtonTitle = SelectedButtonTitle,
					ModifiedSinceMinutesFilter = ModifiedSinceMinutesFilter,
					SelectedMenuItemTitle = SelectedMenuItemTitle,
					StatusFilter = StatusFilter,
					EventTypesFilter = EventTypesFilter
				}
			});

			operation.Run();
        }
    }
}
