/**
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
﻿using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that inserts new node as child to specified one.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class InsertChildNodeAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xpath to the searched node.
        /// </summary>
        private readonly string _xpath;

        /// <summary>
        /// The new node as a XML string.
        /// </summary>
        private readonly string _xmlString;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertBeforeNodeAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node before which we want to add a new node.</param>
        /// <param name="xmlString">The new node as a XML string.</param>
        public InsertChildNodeAction(ILogger logger, ISHFilePath filePath, string xpath, string xmlString)
            : base(logger, filePath)
        {
            _xpath = xpath;
            _xmlString = xmlString;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            XmlConfigManager.InsertChildNode(FilePath, _xpath, _xmlString);
        }
    }
}
