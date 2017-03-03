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

using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that inserts or updates the element of UI.
    /// </summary>
    public class SetElementAction : SingleXmlFileAction
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
        private readonly BaseXMLElement _model;

        /// <summary>
        /// Ban update and generate an exception.
        /// </summary>
        private readonly bool _banUpdateAndGenerateException;

        /// <summary>
        /// The error message.
        /// </summary>
        private readonly string _exceptionMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetElementAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path to XML file.</param>
        /// <param name="model">The model that represents UI element.</param>
        /// <param name="banUpdateAndGenerateException">Ban update and generate an exception. False by default.></param>
        /// <param name="exceptionMessage">The error message.</param>
        public SetElementAction(ILogger logger,
            ISHFilePath filePath,
            BaseXMLElement model, 
            bool banUpdateAndGenerateException = false, 
            string exceptionMessage = "") :
            base(logger, filePath)
        {
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _filePath = filePath;
            _model = model;
            _banUpdateAndGenerateException = banUpdateAndGenerateException;
            _exceptionMessage = exceptionMessage;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _xmlConfigManager.InsertOrUpdateElement(
                _filePath.AbsolutePath,
                _model,
                _banUpdateAndGenerateException,
                _exceptionMessage);
        }
    }
}
