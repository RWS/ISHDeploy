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
﻿using ISHDeploy.Common;
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;

namespace ISHDeploy.Data.Actions.TextFile
{
    /// <summary>
    /// The action is responsible for replace text that matches the saarch pattern.
    /// </summary>
    /// <seealso cref="SingleFileAction" />
    public class TextReplaceAction : SingleFileAction
    {
        /// <summary>
        /// The searched placeholders.
        /// </summary>
        private readonly string _searchPattern;

        /// <summary>
        /// The searched placeholders.
        /// </summary>
        private readonly string _newValue;

        /// <summary>
        /// The text configuration manager.
        /// </summary>
        private readonly ITextConfigManager _textConfigManager;

        /// <summary>
        /// Initializes new instance of the <see cref="CommentBlockAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="searchPattern">Searched pattern</param>
        /// <param name="newValue">The new value</param>  
        public TextReplaceAction(ILogger logger, ISHFilePath filePath, string searchPattern, string newValue)
            : base(logger, filePath)
        {

            _searchPattern = searchPattern;
            _newValue = newValue;
            _textConfigManager = ObjectFactory.GetInstance<ITextConfigManager>();
        }
        
        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _textConfigManager.TextReplace(FilePath, _searchPattern, _newValue);
        }
    }
}
