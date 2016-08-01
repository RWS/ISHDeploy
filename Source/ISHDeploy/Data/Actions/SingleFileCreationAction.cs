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

namespace ISHDeploy.Data.Actions
{
    /// <summary>
	/// Does single file operations that create new file.
	/// </summary>
    public abstract class SingleFileCreationAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The file path.
        /// </summary>
        protected string FilePath;

        /// <summary>
        /// The file manager.
        /// </summary>
        protected readonly IFileManager FileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleFileCreationAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        protected SingleFileCreationAction(ILogger logger, string filePath)
            : base(logger)
        {
            FilePath = filePath;
            FileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
		/// Does nothing for this kind of actions.
		/// </summary>
        public void Backup()
        {
            // for this kind of operation no backup is needed
        }

        /// <summary>
        /// Reverts an asset to initial state.
        /// </summary>
        public void Rollback()
        {
            Logger.WriteDebug($"[{FilePath}][Revert changes");
            if (FileManager.FileExists(FilePath))
            {
                FileManager.Delete(FilePath);
                Logger.WriteDebug($"File {FilePath} removed.");
            }
            Logger.WriteVerbose($"[{FilePath}][Reverted]");
        }
    }
}
