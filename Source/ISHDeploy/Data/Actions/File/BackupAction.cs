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
using System;
using System.IO;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Action responsible for copying directory content from source to destination directory.
	/// </summary>
    /// <seealso cref="BaseAction" />
    public class BackupAction : BaseAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The destination folder path.
        /// </summary>
        private readonly string _destinationFolderPath;
        
        /// <summary>
        /// The source folder path.
        /// </summary>
        private readonly string _sourceFolderPath;

        /// <summary>
        /// Define what will be copied.
        /// </summary>
        private readonly string _template;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyDirectoryAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourceFolderPath">Source folder.</param>
        /// <param name="destinationFolderPath">Destination folder.</param>
        /// <param name="template">Define what will be copied.</param>
        public BackupAction(ILogger logger, string sourceFolderPath, string destinationFolderPath, string template)
            : base(logger)
        {
            _sourceFolderPath = sourceFolderPath;
            _destinationFolderPath = destinationFolderPath;
            _template = template;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _fileManager.CopyWithTemplate(_sourceFolderPath, _destinationFolderPath, _template);
        }
    }
}
