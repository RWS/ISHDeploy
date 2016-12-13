/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using ISHDeploy.Business.Operations.ISHUIElement;
using ISHDeploy.Models.UI;
using System.Collections.Generic;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIEventMonitorMenuBarItem
{
    /// <summary>
    ///		<para type="synopsis">Update or add a new EventMonitor tab.</para>
    ///		<para type="description">The Set-ISHUIEventMonitorMenuBarItem cmdlet updates or adds new Tab definitions to Content Manager deployment.</para>
    ///		<para type="description">If Icon is not specified, the default value '~/UIFramework/events.32x32.png' is taken.</para>
    ///		<para type="description">If UserRole is not specified, the default value 'Administrator' is taken.</para>
    ///		<para type="description">If ModifiedSinceMinutesFilter is not specified, the default value '1440' is taken.</para>
    ///		<para type="description">If SelectedStatusFilter is not specified, the default value 'Recent' is taken.</para>
    ///		<para type="link">Move-ISHUIEventMonitorMenuBarItem</para>
    ///		<para type="link">Remove-ISHUIEventMonitorMenuBarItem</para>
    /// </summary>
    /// <example>
    ///		<code>PS C:\>Set-ISHUIEventMonitorMenuBarItem -ISHDeployment $deployment -Label "All Parameters" -Icon "~/Custom/Images/Custom.png" -EventTypesFilter @("EXPORTFORPUBLICATION", "EXPORTFORPUBLICATIONPDF", "EXPORTFORPUBLICATIONZIP") -SelectedStatusFilter "All" -ModifiedSinceMinutesFilter "3600" -UserRole @("Administrator","Author") -Description "Tab using all available parameters"</code>
    ///		<para>Sets new tab with all sets of available and provided parameters.</para>
    ///		<para>This command sets XML definitions to EventMonitor.
    ///			Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    /// <example>
    ///		<code>PS C:\>Set-ISHUIEventMonitorMenuBarItem -ISHDeployment $deployment -Label "Defaults" -Description "Using default parameters" -UserRole @("Administrator","Author")</code>
    ///		<para>Sets new tab with default set of provided parameters.</para>
    ///		<para>This command sets XML definitions to EventMonitor.
    ///			Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>/// 
    [Cmdlet(VerbsCommon.Set, "ISHUIEventMonitorMenuBarItem")]
    [AdministratorRights]
    public class SetISHUIEventMonitorMenuBarItemCmdlet : BaseHistoryEntryCmdlet
    {
		/// <summary>
		/// Status filter enum
		///	<para type="description">Enumeration of status filters.</para>
		/// </summary>
		public enum StatusFilter
		{
			/// <summary>
			/// Show busy tasks
			/// </summary>
			Busy,

			/// <summary>
			/// Show success tasks
			/// </summary>
			Recent,

			/// <summary>
			/// Show tasks with warnings
			/// </summary>
			Warning,

			/// <summary>
			/// Show failed
			/// </summary>
			Failed,

			/// <summary>
			/// Show All
			/// </summary>
			All
		}

		/// <summary>
		/// The status filter descriptions
		/// </summary>
		private readonly Dictionary<StatusFilter, string> _statusFilterDesctiptions = new Dictionary<StatusFilter, string>
		{
			{ StatusFilter.Recent, "Show Recent" },
			{ StatusFilter.Failed, "Show Failed"},
			{ StatusFilter.Busy, "Show Busy"},
			{ StatusFilter.Warning, "Show Warning"},
			{ StatusFilter.All, "Show All"}
		};

		/// <summary>
		/// <para type="description">Label of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Label of menu item")]
		[ValidateNotNullOrEmpty]
		public string Label { get; set; }

		/// <summary>
		/// <para type="description">Menu item icon representation. Default value is '~/UIFramework/events.32x32.png'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item icon representation")]
		[ValidateNotNullOrEmpty]
		public string Icon { get; set; } = "~/UIFramework/events.32x32.png";

		#region Action parameters

		/// <summary>
		/// <para type="description">Status filter. Null by default</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Status filter")]
		public string[] EventTypesFilter { get; set; } = null;

		/// <summary>
		/// <para type="description">Selected Status filter. Default value is 'Recent'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Selected Status filter")]
		public StatusFilter SelectedStatusFilter { get; set; } = StatusFilter.Recent;

		/// <summary>
		/// <para type="description">Modified since minutes filter value. Default value is '1440'.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Modified since minutes filter value")]
		public int ModifiedSinceMinutesFilter { get; set; } = 1440;

		#endregion

		/// <summary>
		/// <para type="description">Can be applied for roles.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Action of menu item")]
		[ValidateNotNullOrEmpty]
        public string[] UserRole { get; set; }

		/// <summary>
		/// <para type="description">User role description.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "User role description")]
		public string Description { get; set; }

		/// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var model = new EventMonitorMenuBarItem(
                Label,
                UserRole, // we need single form in powershell
                Icon = Icon,
                new Models.ISHXmlNodes.EventLogMenuItemAction()
                {
                    SelectedButtonTitle = _statusFilterDesctiptions[SelectedStatusFilter],
                    ModifiedSinceMinutesFilter = ModifiedSinceMinutesFilter,
                    SelectedMenuItemTitle = Label,
                    StatusFilter = StatusFilter.All.ToString(), // By default 'All' is used
                    EventTypesFilter = EventTypesFilter
                }.ToQueryString(),
                Description
			);

            new SetUIElementOperation(Logger, ISHDeployment, model).Run();
        }
    }
}
