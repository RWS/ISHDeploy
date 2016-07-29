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
using System.Security.AccessControl;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.File
{

    /// <summary>
	/// Assigns the necessary permissions to a path for a user
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class FileSystemRightsAssignAction : BaseAction
    {
        /// <summary>
        /// File system access rights
        /// </summary>
        public enum FileSystemAccessRights
        {
            /// <summary>
            /// The full control
            /// </summary>
            FullControl
        }

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The path to folder or file
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The user
        /// </summary>
        private readonly string _user;

        /// <summary>
        /// The user rights
        /// </summary>
        private readonly FileSystemRights _rights;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemRightsAssignAction"/> class.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="path">The path to folder or file</param>
        /// <param name="user">The user</param>
        /// <param name="accessRights">The user access rights</param>
        public FileSystemRightsAssignAction(ILogger logger, string path, string user, FileSystemAccessRights accessRights) 
			: base(logger)
        {
            _path = path;
            _user = user;
            _rights = FileSystemRights.Read;

            if (accessRights == FileSystemAccessRights.FullControl)
            {
                _rights = FileSystemRights.FullControl;
            }

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            if (_fileManager.FolderExists(_path))
            {
                _fileManager.AssignPermissionsForDirectory(_path, _user, _rights,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None,
                    AccessControlType.Allow);
            }
            else if (_fileManager.FileExists(_path))
            {
                _fileManager.AssignPermissionsForFile(_path, _user, _rights, AccessControlType.Allow);
            }
            else
            {
                throw new Exception($"Path not found: {_path}");
            }
		}
	}
}
