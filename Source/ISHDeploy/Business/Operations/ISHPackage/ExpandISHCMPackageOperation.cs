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
    /// 
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
            _invoker = new ActionInvoker(logger, "Extract ISHCM files.");


            #region Ensure the list of vanilla files has been saved as file

            if (!_fileManager.FileExists(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath))
            {
                _fileManager.EnsureDirectoryExists(BackupFolderPath);

                var fullFileList = _fileManager.GetFileSystemEntries(
                    AuthorAspBinFolderPath, "*.*", SearchOption.AllDirectories);

                ObjectFactory.GetInstance<IXmlConfigManager>()
                    .SerializeToFile(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath, fullFileList);
            }

            #endregion

            Array.ForEach(zipFilePathArray, zipFilePath =>
            {
                var absoluteZipFilePath = Path.Combine(PackagesFolderPath, zipFilePath);

                if (!_fileManager.FileExists(absoluteZipFilePath))
                {
                    throw new ArgumentException($"InvalidPath for {absoluteZipFilePath} file.");
                }

                if (toBinary)
                {
                    var doc = _fileManager.Load(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath);

                    var filesList = doc
                               .Element("ArrayOfString")
                               .Elements("string")
                               .Select(x => x.Value.Substring(x.Value.IndexOf(@"\bin\") + 5).Replace("\\", "/"));

                    ExtractZipFile(zipFilePath, AuthorAspBinFolderPath, filesList);
                }
                else
                {
                    ExtractZipFile(zipFilePath, AuthorAspCustomFolderPath);
                }
            });
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

                    if (ignoreFiles != null && ignoreFiles.Any(y => y == x.FullName))
                    {
                        Logger.WriteWarning($"Skip file {x}, because it present in vanilla version.");
                        return;
                    }

                    string destinationFilePath = $"{destinationDirectory}/{x}";

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