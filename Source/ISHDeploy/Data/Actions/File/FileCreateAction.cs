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
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Action that creates new file with content inside.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class FileCreateAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;


        /// <summary>
        /// The file content
        /// </summary>
        private readonly string _fileContent;

        /// <summary>
        /// The destination path for new file.
        /// </summary>
        private readonly string _destinationPath;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCreateAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="fileContent">Content of the new file.</param>
        public FileCreateAction(ILogger logger, string destinationPath, string fileContent) 
			: base(logger)
        {
			_fileContent = fileContent;
			_destinationPath = destinationPath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
			_fileManager.Write(_destinationPath, _fileContent);
		}

        /// <summary>
        /// Reverts an asset to initial state
        /// </summary>
        public virtual void Rollback()
		{
			Logger.WriteDebug($"Rolling back file creatiog {_destinationPath}.");
			if (_fileManager.FileExists(_destinationPath))
			{
				_fileManager.Delete(_destinationPath);
			}
		}

        /// <summary>
        /// Creates backup of the asset
        /// </summary>
        public void Backup()
		{
            // This actions does not require backup
            //	So do nothing here
        }
	}
}
