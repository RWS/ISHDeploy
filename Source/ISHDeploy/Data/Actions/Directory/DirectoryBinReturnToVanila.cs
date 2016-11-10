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
using System.IO;
using System.Linq;

namespace ISHDeploy.Data.Actions.Directory
{
    /// <summary>
	/// Action responsible for removing directory.
    /// </summary>
    /// <seealso cref="BaseAction" />
    public class DirectoryBinReturnToVanila : BaseAction
    {
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The folder to be removed
        /// </summary>
        private readonly string _folderPath;

        /// <summary>
        /// The file with list to be leaved the rest will be removed
        /// </summary>
        private readonly string _backupFile;

        /// <summary>
        /// Path to BIN folder
        /// </summary>
        private readonly string _binFolderPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryRemoveAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="folderPath">The folder that will be removed.</param>
        /// <param name="backupFile">The file with list to be leaved the rest will be removed.</param>
        /// <param name="binFolderPath">Path to BIN folder.</param>
        public DirectoryBinReturnToVanila(ILogger logger, string folderPath, string backupFile, string binFolderPath)
            : base(logger)
        {
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _folderPath = folderPath;
            _backupFile = backupFile;
            _binFolderPath = binFolderPath;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            string fullBackupFile = Path.Combine(_folderPath, _backupFile);
            if (_fileManager.FileExists(fullBackupFile))
            {
                var doc = _fileManager.Load(fullBackupFile);

                System.IO.Directory
                    .GetFileSystemEntries(_binFolderPath, "*.*", SearchOption.AllDirectories)
                    .Except(doc
                            .Element("ArrayOfString")
                            .Elements("string")
                            .Select(x => x.Value))
                    .ToList()
                    .ForEach(x =>
                    {
                        if (System.IO.Directory.Exists(x))
                            System.IO.Directory.Delete(x, true);
                        if (System.IO.File.Exists(x))
                            System.IO.File.Delete(x);
                    });
            }
        }
    }
}
