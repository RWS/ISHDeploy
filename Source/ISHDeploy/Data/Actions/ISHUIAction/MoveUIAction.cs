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
using ISHDeploy.Business.Enums;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI;

namespace ISHDeploy.Data.Actions.ISHUIAction
{
    /// <summary>
    /// Remove xml node from file.
    /// </summary>
    public class MoveUIAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xml configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        private string _filePath;
        private BaseUIModel _model;
        private OperationType _operation;
        private string _after;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveUIAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public MoveUIAction(ILogger logger,
            ISHFilePath filePath,
            BaseUIModel model, 
            OperationType operation,
            string after) :
            base(logger, filePath)
        {
            _filePath = filePath.AbsolutePath;
            _model = model;
            _operation = operation;
            _after = after;
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }

        public override void Execute()
        {
            _xmlConfigManager.MoveElement(
                _filePath,
                _model,
                _operation,
                _after);
        }
    }
}
