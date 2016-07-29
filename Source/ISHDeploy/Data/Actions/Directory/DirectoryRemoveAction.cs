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

namespace ISHDeploy.Data.Actions.Directory
{
    /// <summary>
	/// Action responsible for removing directory.
    /// </summary>
    /// <seealso cref="BaseAction" />
    public class DirectoryRemoveAction : BaseAction
	{
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The folder to be removed
        /// </summary>
        private readonly string _folder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryRemoveAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
		/// <param name="folderPath">The folder that will be removed.</param>
        public DirectoryRemoveAction(ILogger logger, string folderPath)
			: base(logger)
		{
			_fileManager = ObjectFactory.GetInstance<IFileManager>();
			_folder = folderPath;
		}

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
			_fileManager.DeleteFolder(_folder);
		}
	}
}
