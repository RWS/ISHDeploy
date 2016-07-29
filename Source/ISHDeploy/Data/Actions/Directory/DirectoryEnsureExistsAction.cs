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
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.Directory
{
    /// <summary>
	/// Action that creates new folder.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class DirectoryEnsureExistsAction : BaseAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The path to folder that will be created.
        /// </summary>
        private readonly string _folderPath;


        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryEnsureExistsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="folderPath">The path to folder that will be created.</param>
        public DirectoryEnsureExistsAction(ILogger logger, string folderPath) 
			: base(logger)
        {
            _folderPath = folderPath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
			_fileManager.EnsureDirectoryExists(_folderPath);
		}
	}
}
