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
﻿using System;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
	/// Action that waits until file becomes unlocked.
    /// </summary>
    /// <seealso cref="BaseAction" />
    public class FileWaitUnlockAction : BaseAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The path to the file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCreateAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The path to the file.</param>
        public FileWaitUnlockAction(ILogger logger, ISHFilePath filePath) 
			: base(logger)
        {
            _filePath = filePath.AbsolutePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
		{
            int i = 0;
            while (_fileManager.IsFileLocked(_filePath))
            {
                System.Threading.Thread.Sleep(100);
                i++;

                if (i > 100)
                {
                    throw new TimeoutException($"The process cannot access the file '{_filePath}' for a long time because it is being used by another process");
                }
            }
        }
        
	}
}
