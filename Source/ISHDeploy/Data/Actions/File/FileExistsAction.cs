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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
	/// <summary>
	/// Checks file exist.
	/// </summary>
    public class FileExistsAction : BaseActionWithResult<bool>
    {
        /// <summary>
        /// The file path.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExistsAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="returnResult">The delegate that returns all text of file.</param>
        public FileExistsAction(ILogger logger, string filePath, Action<bool> returnResult)
            : base(logger, returnResult)
        {
            _filePath = filePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>File content</returns>
        protected override bool ExecuteWithResult()
        {
            return _fileManager.FileExists(_filePath);
        }
    }
}
