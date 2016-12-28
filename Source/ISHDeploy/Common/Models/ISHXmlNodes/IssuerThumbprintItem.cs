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
﻿using System.Xml.Linq;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Common.Models.ISHXmlNodes
{
	/// <summary>
	/// Represents menu item xml node
	/// </summary>
	public class IssuerThumbprintItem : IISHXmlNode
	{
		/// <summary>
		/// Xml node name.
		/// </summary>
		protected virtual string XmlElementName => "add";

		/// <summary>
		/// Gets or sets the thumbprint.
		/// </summary>
		/// <value>
		/// The thumbprint value.
		/// </value>
		public string Thumbprint { get; set; }

		/// <summary>
		/// Gets or sets the Issuer name.
		/// </summary>
		/// <value>
		/// The Issuer name value.
		/// </value>
		public string Issuer { get; set; }

		/// <summary>
		/// Gets node comment if exists
		/// </summary>
		public XComment GetNodeComment()
		{
			return null;
		}

		/// <summary>
		/// Converts object to XElement.
		/// </summary>
		/// <returns>XElement</returns>
		public virtual XElement ToXElement()
		{
			return new XElement(XmlElementName,
				new XAttribute("thumbprint", Thumbprint),
				new XAttribute("name", Issuer));
		}
	}
}
