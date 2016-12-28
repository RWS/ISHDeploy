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
﻿using System.IO;
﻿using ISHDeploy.Common;
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Implements file copy to directory action
    /// </summary>
    public class FileCopyToDirectoryAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The source file path
        /// </summary>
        private readonly string _sourcePath;

        /// <summary>
        /// The path to destination folder
        /// </summary>
        private readonly string _destinationDirectory;

        /// <summary>
        /// The force switch identifies if file needs to be replaced.
        /// </summary>
        private readonly bool _force;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyToDirectoryAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationDirectory">The path to destination folder.</param>
        /// <param name="force">Replaces existing file if true.</param>
        public FileCopyToDirectoryAction(ILogger logger, string sourcePath, string destinationDirectory, bool force = false) 
			: base(logger)
        {
            _sourcePath = sourcePath;
			_destinationDirectory = destinationDirectory;
            _force = force;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
			_fileManager.CopyToDirectory(_sourcePath, _destinationDirectory, _force);
		}

        /// <summary>
        /// Reverts an asset to initial state.
        /// </summary>
        public virtual void Rollback()
		{
			var fileName = Path.GetFileName(_sourcePath);

			Logger.WriteVerbose($"Rolling back file `{fileName}` copy action.");
			if (string.IsNullOrEmpty(fileName))
		    {
		        return;
		    }

			var copiedFileName = Path.Combine(_destinationDirectory, fileName);
			if (_fileManager.FileExists(copiedFileName))
		    {
				_fileManager.Delete(copiedFileName);
			}
		}

		/// <summary>
		/// Used to create a backup of the file, however, as this command is doing no modification 
		/// on existing file we keep this method empty
		/// </summary>
		public void Backup()
		{
			//	Otherwise backup means removing added item
			//	So do nothing here
		}
	}
}
