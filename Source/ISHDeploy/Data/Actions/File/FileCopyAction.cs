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
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.File
{
	/// <summary>
	/// Implements file copy action
	/// </summary>
    public class FileCopyAction : BaseAction
    {
        /// <summary>
        /// The source file path
        /// </summary>
        private readonly string _sourcePath;

        /// <summary>
        /// The destination file path
        /// </summary>
        private readonly string _destinationPath;

        /// <summary>
        /// The force switch identifies if file needs to be replaced.
        /// </summary>
        private readonly bool _force;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="force">Replaces existing file if true.</param>
        public FileCopyAction(ILogger logger, string sourcePath, string destinationPath, bool force = false) 
			: base(logger)
        {
            _sourcePath = sourcePath;
			_destinationPath = destinationPath;
            _force = force;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
			_fileManager.Copy(_sourcePath, _destinationPath, _force);
		}
	}
}
