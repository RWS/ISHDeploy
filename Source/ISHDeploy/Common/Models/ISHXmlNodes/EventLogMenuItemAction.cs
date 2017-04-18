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
﻿using System;

namespace ISHDeploy.Common.Models.ISHXmlNodes
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
