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
    /// <seealso cref="SingleXmlFileAction" />
    public class SetAttributeValueAction : SingleXmlFileAction
    {
        /// <summary>
        /// The attribute xPath.
        /// </summary>
        private readonly string _attributeXpath;

        /// <summary>
        /// The attribute value.
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// Create attribute if not exist.
        /// </summary>
        private readonly bool _createAttributeIfNotExist;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAttributeValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The attribute new value.</param>
        /// <param name="createAttributeIfNotExist">Create attribute if not exist.</param>
        public SetAttributeValueAction(ILogger logger, ISHFilePath filePath, string xpath, string attributeName, string value, bool createAttributeIfNotExist = false)
            : this(logger, filePath, string.Concat(xpath, "/@", attributeName), value, createAttributeIfNotExist)
		{ }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAttributeValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="attributeXpath">The xpath to the node.</param>
        /// <param name="value">The attribute new value.</param>
        /// <param name="createAttributeIfNotExist">Create attribute if not exist.</param>
        public SetAttributeValueAction(ILogger logger, ISHFilePath filePath, string attributeXpath, string value, bool createAttributeIfNotExist = false)
			: base(logger, filePath)
		{
			_attributeXpath = attributeXpath;
			_value = value;
            _createAttributeIfNotExist = createAttributeIfNotExist;
		}

		/// <summary>
		/// Executes current action.
		/// </summary>
		public override void Execute()
        {
            XmlConfigManager.SetAttributeValue(FilePath, _attributeXpath, _value, _createAttributeIfNotExist);
        }
    }
}
