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
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Action responsible for copying directory content from source to destination directory.
	/// </summary>
    /// <seealso cref="BaseAction" />
    public class FileCopyDirectoryAction : BaseAction
	{
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The source folder path.
        /// </summary>
        private readonly string _sourceFolder;

        /// <summary>
        /// The destination folder path.
        /// </summary>
        private readonly string _destinationFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyDirectoryAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourceFolder">Source folder.</param>
        /// <param name="destinationFolder">Destination folder.</param>
        public FileCopyDirectoryAction(ILogger logger, string sourceFolder, string destinationFolder)
            : base(logger)
        {
			_sourceFolder = sourceFolder;
			_destinationFolder = destinationFolder;

			_fileManager = ObjectFactory.GetInstance<IFileManager>();
		}

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
	    {
			_fileManager.CopyDirectoryContent(_sourceFolder, _destinationFolder);
		}
	}
}
