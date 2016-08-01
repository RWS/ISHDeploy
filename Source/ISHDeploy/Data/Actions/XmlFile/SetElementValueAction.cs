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
﻿using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that sets new value for specified element.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class SetElementValueAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xpath to the searched element.
        /// </summary>
        private readonly string _xpath;

        /// <summary>
        /// The new value of element.
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetElementValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the element.</param>
        /// <param name="value">The new value of element.</param>
        public SetElementValueAction(ILogger logger, ISHFilePath filePath, string xpath, string value)
            : base(logger, filePath)
        {
            _xpath = xpath;
            _value = value;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            XmlConfigManager.SetElementValue(FilePath, _xpath, _value);
        }
    }
}
