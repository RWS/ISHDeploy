using System;
using System.Xml.Linq;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Models.ISHXmlNodes
{
	/// <summary>
	/// Represents menu item xml node
	/// </summary>
	public class EventLogMenuItem : IISHXmlNode
    {
        /// <summary>
        /// The Xopus add check out comment placeholder
        /// </summary>
        public const string EventMonitorTabCommentMarkup = " {0} ================================== ";

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

		/// <summary>
		/// Gets or sets the action.
		/// </summary>
		public EventLogMenuItemAction Action { get; set; }

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		public string Icon { get; set; }

		/// <summary>
		/// Gets or sets the userrole.
		/// </summary>
		public string UserRole { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets node comemnt if exiss
		/// </summary>
		public XComment GetNodeComment()
		{
			var commentLabel = Description ?? Label;
			if (!String.IsNullOrEmpty(commentLabel))
			{
				return new XComment(string.Format(EventMonitorTabCommentMarkup, commentLabel));
			}

			return null;
		}

		/// <summary>
		/// Converts object to XElement.
		/// </summary>
		/// <returns>XElement</returns>
		public XElement ToXElement()
		{
			return new XElement("menuitem",
				new XAttribute("label", Label),
				new XAttribute("action", Action.ToQueryString()),
				new XAttribute("icon", Icon),
				new XElement("userrole", UserRole),
				new XElement("description", Description));
		}
	}
}
