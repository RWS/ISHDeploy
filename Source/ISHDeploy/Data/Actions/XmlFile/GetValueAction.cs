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

using System;
using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that gets new value from specified element.
    /// </summary>
    /// <seealso cref="GetValueAction" />
    public class GetValueAction : BaseActionWithResult<string>
    {
        /// <summary>
        /// The XML configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// The path to the file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The xpath to the searched element.
        /// </summary>
        private readonly string _xpath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetElementValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the element.</param>
        /// <param name="returnResult">The delegate that returns value.</param>
        public GetValueAction(ILogger logger, ISHFilePath filePath, string xpath, Action<string> returnResult)
            : base(logger, returnResult)
        {
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _filePath = filePath.AbsolutePath;
            _xpath = xpath;
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns></returns>
        protected override string ExecuteWithResult()
        {
            return _xmlConfigManager.GetValue(_filePath, _xpath);
        }
    }
}
