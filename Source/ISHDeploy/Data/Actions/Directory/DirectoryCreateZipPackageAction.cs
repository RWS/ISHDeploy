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
﻿using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Directory
{
    /// <summary>
    /// Saves files to package.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class DirectoryCreateZipPackageAction : SingleFileCreationAction
    {
        /// <summary>
        /// The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.
        /// </summary>
        private readonly string _destinationArchiveFilePath;

        /// <summary>
        /// The parameter determines whether to include subfolders to the archive
        /// </summary>
        private readonly bool _includeBaseDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCreateZipPackageAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourcePath">The path to the directory to be archived, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="destinationArchiveFilePath">The path of the archive to be created, specified as a relative or absolute path. A relative path is interpreted as relative to the current working directory.</param>
        /// <param name="includeBaseDirectory">'True' to include the directory name from sourceDirectoryName at the root of the archive; 'False' to include only the contents of the directory. 'False' by default</param>
        public DirectoryCreateZipPackageAction(ILogger logger, string sourcePath, string destinationArchiveFilePath,
            bool includeBaseDirectory = false)
            : base(logger, sourcePath)
        {
            _destinationArchiveFilePath = destinationArchiveFilePath;
            _includeBaseDirectory = includeBaseDirectory;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            FileManager.PackageDirectory(FilePath, _destinationArchiveFilePath, _includeBaseDirectory);
        }
    }
}
