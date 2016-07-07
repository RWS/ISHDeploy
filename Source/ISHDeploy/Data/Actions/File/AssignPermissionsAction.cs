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
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Assigns the necessary permissions to a path for a user
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class AssignPermissionsAction : BaseAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The path to folder
        /// </summary>
        private readonly string _folderPath;

        /// <summary>
        /// The user
        /// </summary>
        private readonly string _user;


        /// <summary>
        /// Initializes a new instance of the <see cref="AssignPermissionsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="folderPath">The path to folder</param>
        /// <param name="user">The user</param>
        public AssignPermissionsAction(ILogger logger, string folderPath, string user) 
			: base(logger)
        {
            _folderPath = folderPath;
            _user = user;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
			_fileManager.AssignPermissions(_folderPath, _user);
		}
	}
}
