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

using System.IO;
using System.Linq;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// Extracts zip files to bin or custom folder depends on toBinary flag
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class ExpandISHCMPackageOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// If ToBinary switched
        /// </summary>
        private readonly bool _toBinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandISHCMPackageOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="zipFilePathArray">Array of zip files.</param>
        /// <param name="toBinary">If ToBinary switched.</param>
        public ExpandISHCMPackageOperation(ILogger logger, Models.ISHDeployment ishDeployment, string[] zipFilePathArray, bool toBinary = false) :
            base(logger, ishDeployment)
        {
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _toBinary = toBinary;

            _invoker = new ActionInvoker(logger, "Extract ISHCM files.");

            #region Ensure the list of vanilla files has been saved as file

            if (_toBinary && !_fileManager.FileExists(VanillaFilesOfWebAuthorAspBinFolderFilePath))
            {
                _fileManager.EnsureDirectoryExists(BackupFolderPath);

                var fullFileList = _fileManager.GetFileSystemEntries(
                    AuthorAspBinFolderPath, "*.*", SearchOption.AllDirectories);

                xmlConfigManager.SerializeToFile(VanillaFilesOfWebAuthorAspBinFolderFilePath, fullFileList);
            }

            #endregion

            string[] listOfIgnoreFilesInBinFolder = _toBinary ? xmlConfigManager.Deserialize<string[]>(VanillaFilesOfWebAuthorAspBinFolderFilePath) : null;

            var inputParameters =
                xmlConfigManager.GetAllInputParamsValues(InputParametersFilePath.AbsolutePath);

            var unzippedFolders = new List<string>();
            Array.ForEach(zipFilePathArray, zipFilePath =>
            {
                var absoluteZipFilePath = Path.Combine(PackagesFolderPath, zipFilePath);
                var unzipFolderPath = Path.Combine(PackagesFolderPath, $"{Path.GetFileName(absoluteZipFilePath).Replace(".", string.Empty)}_{Guid.NewGuid()}");

                if (!_fileManager.FileExists(absoluteZipFilePath))
                {
                    throw new ArgumentException($"InvalidPath for {absoluteZipFilePath} file.");
                }

                IEnumerable<string> unzippedFiles = ExtractZipFile(absoluteZipFilePath, unzipFolderPath);

                unzippedFiles?.ToList().ForEach(unzippedFilePath =>
                {
                    var destinationFilePath = _toBinary
                        ? unzippedFilePath.Replace(unzipFolderPath, AuthorAspBinFolderPath)
                        : unzippedFilePath.Replace(unzipFolderPath, AuthorAspCustomFolderPath);


                    if (_toBinary && listOfIgnoreFilesInBinFolder != null && listOfIgnoreFilesInBinFolder.Any(y => y == destinationFilePath))
                    {
                        _invoker.AddAction(new WriteWarningAction(Logger, () => (true),
                            $"Skip file {destinationFilePath}, because it present in vanilla version."));
                    }
                    else
                    {
                        bool destinationFileExists = _fileManager.FileExists(destinationFilePath);

                        _invoker.AddAction(new FileCopyAndReplacePlaceholdersAction(Logger, unzippedFilePath,
                            destinationFilePath,
                            inputParameters));

                        _invoker.AddAction(new WriteWarningAction(Logger, () => (destinationFileExists),
                            $"File {destinationFilePath} has been overritten."));
                    }
                });

                unzippedFolders.Add(unzipFolderPath);
            });

            unzippedFolders.ForEach(x => _invoker.AddAction(new DirectoryRemoveAction(Logger, x)));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }

        /// <summary>
        /// Extracts file to specified folder
        /// </summary>
        /// <param name="zipFilePath">The path to zip file.</param>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <returns>Liest of paths to extracted files</returns>
        private List<string> ExtractZipFile(string zipFilePath, string destinationDirectory)
        {
            var unzippedFiles = new List<string>();

            using (var archive = ZipFile.OpenRead(zipFilePath))
            {
                var files = archive.Entries.ToList();

                files
                .ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.Name)) return;

                    string destinationFilePath = Path.Combine(destinationDirectory, x.FullName.Replace("/", "\\"));

                    string destinationFolderPath = Path.GetDirectoryName(destinationFilePath);
                    _fileManager.EnsureDirectoryExists(destinationFolderPath);

                    x.ExtractToFile(destinationFilePath, true);
                    unzippedFiles.Add(destinationFilePath);
                });
            }

            return unzippedFiles;
        }
    }
}