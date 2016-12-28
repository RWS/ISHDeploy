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

using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
	/// <summary>
	/// Implements file delete action.
	/// </summary>
    public class FileDeleteAction : BaseAction
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
        /// Initializes a new instance of the <see cref="FileDeleteAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        public FileDeleteAction(ILogger logger, string filePath) 
			: base(logger)
        {
            _filePath = filePath;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

		/// <summary>
		/// Executes the action.
		/// </summary>
		public override void Execute()
		{
            if (_fileManager.FileExists(_filePath))
            {
                _fileManager.Delete(_filePath);
            }
		}
	}
}
