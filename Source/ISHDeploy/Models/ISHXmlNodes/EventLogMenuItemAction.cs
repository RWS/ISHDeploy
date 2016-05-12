using System;

namespace ISHDeploy.Models.ISHXmlNodes
{
	/// <summary>
	/// Represents menu item xml node
	/// </summary>
	public class EventLogMenuItemAction
    {
        /// <summary>
        /// The Xopus add check out comment placeholder
        /// </summary>
        public const string EventActionPath = "EventMonitor/Main/Overview?";
        
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string SelectedButtonTitle { get; set; }

		/// <summary>
		/// Gets or sets the action.
		/// </summary>
		public int ModifiedSinceMinutesFilter { get; set; }

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		public string SelectedMenuItemTitle { get; set; }

		/// <summary>
		/// Gets or sets the userrole.
		/// </summary>
		public string StatusFilter { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string[] EventTypesFilter { get; set; }

		/// <summary>
		/// Converts object to XElement.
		/// </summary>
		/// <returns>XElement</returns>
		public string ToQueryString()
		{
			return EventActionPath + string.Join("&", new string[]
			{
				"eventTypesFilter=" + ((EventTypesFilter == null) ? "" : string.Join(", ", EventTypesFilter)),
				"statusFilter=" + StatusFilter,
				"selectedMenuItemTitle=" + SelectedMenuItemTitle,
				"modifiedSinceMinutesFilter=" + ModifiedSinceMinutesFilter,
				"selectedButtonTitle=" + Uri.EscapeUriString(SelectedButtonTitle)
			});
		}
	}
}
