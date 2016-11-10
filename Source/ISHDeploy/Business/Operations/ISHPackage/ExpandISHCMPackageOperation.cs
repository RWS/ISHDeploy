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
using ISHDeploy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using ISHDeploy.Business.Invokers;

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
        /// The xml manager
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// Array of zip files
        /// </summary>
        private readonly string[] _zipFilePathArray;

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
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _zipFilePathArray = zipFilePathArray;
            _toBinary = toBinary;

            _invoker = new ActionInvoker(logger, "Extract ISHCM files.");

            #region Ensure the list of vanilla files has been saved as file

            if (_toBinary && !_fileManager.FileExists(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath))
            {
                _fileManager.EnsureDirectoryExists(BackupFolderPath);

                var fullFileList = _fileManager.GetFileSystemEntries(
                    AuthorAspBinFolderPath, "*.*", SearchOption.AllDirectories);

                _xmlConfigManager.SerializeToFile(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath, fullFileList);
            }

            #endregion
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
            Array.ForEach(_zipFilePathArray, zipFilePath =>
            {
                var absoluteZipFilePath = Path.Combine(PackagesFolderPath, zipFilePath);

                if (!_fileManager.FileExists(absoluteZipFilePath))
                {
                    throw new ArgumentException($"InvalidPath for {absoluteZipFilePath} file.");
                }

                if (_toBinary)
                {
                    var ignoreFiles = _xmlConfigManager.Deserialize<string[]>(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath);

                    ExtractZipFile(absoluteZipFilePath, AuthorAspBinFolderPath, ignoreFiles);
                }
                else
                {
                    ExtractZipFile(absoluteZipFilePath, AuthorAspCustomFolderPath);
                }
            });
        }

        /// <summary>
        /// Extracts file to specified folder
        /// </summary>
        /// <param name="zipFilePath">The path to zip file.</param>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <param name="ignoreFiles">List of files to ignore. Null by default</param>
        private void ExtractZipFile(string zipFilePath, string destinationDirectory, IEnumerable<string> ignoreFiles = null)
        {
            using (var archive = ZipFile.OpenRead(zipFilePath))
            {
                var files = archive.Entries.ToList();

                files
                .ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.Name)) return;

                    string destinationFilePath = Path.Combine(destinationDirectory, x.FullName.Replace("/", "\\"));

                    if (ignoreFiles != null && ignoreFiles.Any(y => y == destinationFilePath))
                    {
                        Logger.WriteWarning($"Skip file {x}, because it present in vanilla version.");
                        return;
                    }

                    string destinatioFolderPath = Path.GetDirectoryName(destinationFilePath);
                    _fileManager.EnsureDirectoryExists(destinatioFolderPath);
                    var present = _fileManager.FileExists(destinationFilePath);

                    x.ExtractToFile(destinationFilePath, true);

                    if (present)
                    {
                        Logger.WriteWarning($"File {destinationFilePath} has been overritten.");
                    }
                });
            }
        }
    }
}