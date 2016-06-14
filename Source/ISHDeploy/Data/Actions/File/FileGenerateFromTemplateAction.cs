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
﻿using System.Collections.Generic;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Saves document generated from resource template.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class FileGenerateFromTemplateAction : SingleFileCreationAction
    {
        /// <summary>
        /// The path to template file.
        /// </summary>
        private readonly string _templateFilePath;

        /// <summary>
        /// The output file path.
        /// </summary>
        private readonly string _outputFilePath;

        /// <summary>
        /// The set of parameters.
        /// </summary>
        private readonly Dictionary<string, string> _parameters;

        /// <summary>
        /// The template manager/
        /// </summary>
        private readonly ITemplateManager _templateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileGenerateFromTemplateAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="templateFilePath">The path to template file.</param>
        /// <param name="parameters">The set of parameters.</param>
        public FileGenerateFromTemplateAction(ILogger logger,
            string templateFilePath,
            string outputFilePath,
            Dictionary<string, string> parameters) : base(logger, outputFilePath)
        {
            _templateManager = ObjectFactory.GetInstance<ITemplateManager>();
            _outputFilePath = outputFilePath;
            _templateFilePath = templateFilePath;
            _parameters = parameters;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var outputContent = _templateManager.GenerateDocument(_templateFilePath, _parameters);

            FileManager.Write(_outputFilePath, outputContent);
        }
    }
}
