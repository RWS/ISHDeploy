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

using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI;

namespace ISHDeploy.Data.Actions.ISHUIElement
{
    /// <summary>
    /// Action that inserts or update the element of UI.
    /// </summary>
    public class SetUIElementAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xml configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// The file path to XML file.
        /// </summary>
        private readonly ISHFilePath _filePath;

        /// <summary>
        /// The model that represents UI element.
        /// </summary>
        private readonly BaseUIElement _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUIElementAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path to XML file.</param>
        /// <param name="model">The model that represents UI element.</param>
        public SetUIElementAction(ILogger logger,
            ISHFilePath filePath,
            BaseUIElement model) :
            base(logger, filePath)
        {
            _filePath = filePath;
            _model = model;
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _xmlConfigManager.InsertOrUpdateUIElement(
                _filePath.AbsolutePath,
                _model);
        }
    }
}
