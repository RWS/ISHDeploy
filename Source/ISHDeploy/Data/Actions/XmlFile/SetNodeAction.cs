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
﻿using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
	/// <summary>
	/// Action that sets specific node attribute to the certain value.
	/// </summary>
	/// <seealso cref="ISHDeploy.Data.Actions.SingleXmlFileAction" />
	/// <seealso cref="SingleXmlFileAction" />
	public class SetNodeAction : SingleXmlFileAction
	{
		/// <summary>
		/// The xpath to the searched node.
		/// </summary>
		private readonly string _xpath;

		/// <summary>
		/// ISH configuration XML Node.
		/// </summary>
		private readonly IISHXmlNode _ishXmlNode;

		/// <summary>
		/// Identifies if existing node should be replaced.
		/// </summary>
		private readonly bool _replaceIfExists;

		/// <summary>
		/// Implements Set node action.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="xpath">The xpath to the node.</param>
		/// <param name="ishXmlNode">The ish XML node.</param>
		/// <param name="replaceIfExists">if set to <c>true</c> replaces existing node if exists, otherwise does nothing.</param>
		public SetNodeAction(ILogger logger, ISHFilePath filePath, string xpath, IISHXmlNode ishXmlNode, bool replaceIfExists = true)
            : base(logger, filePath)
        {
			_xpath = xpath;
			_ishXmlNode = ishXmlNode;
			_replaceIfExists = replaceIfExists;
		}

		/// <summary>
		/// Executes current action.
		/// </summary>
		public override void Execute()
		{
			XmlConfigManager.SetNode(FilePath, _xpath, _ishXmlNode, _replaceIfExists);
		}
	}
}
