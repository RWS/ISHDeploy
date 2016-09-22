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

using ISHDeploy.Business.Enums;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI;

namespace ISHDeploy.Data.Actions.ISHUIElement
{
    /// <summary>
    /// Action that changes order of UI elements.
    /// </summary>
    public class MoveUIElementAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xml configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// The file path to XML file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The model that represents UI element.
        /// </summary>
        private readonly BaseUIElement _model;

        /// <summary>
        /// The direction to move.
        /// </summary>
        private readonly MoveElementDirection _direction;

        /// <summary>
        /// The XPath to element to move after it.
        /// </summary>
        private readonly string _insertAfterXpath;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUIElementAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path to XML file.</param>
        /// <param name="model">The model that represents UI element.</param>
        /// <param name="direction">The direction to move.</param>
        /// <param name="after">The id of element to move after it.</param>
        public MoveUIElementAction(ILogger logger,
            ISHFilePath filePath,
            BaseUIElement model, 
            MoveElementDirection direction,
            string after = null) :
            base(logger, filePath)
        {
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _filePath = filePath.AbsolutePath;
            _model = model;
            _direction = direction;

            if (!string.IsNullOrEmpty(after))
            {
                _insertAfterXpath = string.Format(model.XPathFormat, after);
            }
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _xmlConfigManager.MoveUIElement(
                _filePath,
                _model,
                _direction,
                _insertAfterXpath);
        }
    }
}
