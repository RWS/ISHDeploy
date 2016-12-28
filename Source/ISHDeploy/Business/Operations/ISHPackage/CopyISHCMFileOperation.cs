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
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using Models = ISHDeploy.Common.Models;
using ISHDeploy.Common;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class CopyISHCMFileOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyISHCMFileOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="files">Relative path to file.</param>
        /// <param name="toBinary">If ToBinary switched.</param>
        public CopyISHCMFileOperation(ILogger logger, Models.ISHDeployment ishDeployment, string[] files, bool toBinary = false) :
            base(logger, ishDeployment)
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _invoker = new ActionInvoker(logger, "Copy ISHCM files.");

            #region Ensure the list of vanilla files has been saved as file

            if (toBinary && !fileManager.FileExists(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath))
            {
                fileManager.EnsureDirectoryExists(BackupFolderPath);

                var fullFileList = fileManager.GetFileSystemEntries(
                    AuthorAspBinFolderPath, "*.*", SearchOption.AllDirectories);

                xmlConfigManager.SerializeToFile(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath, fullFileList);
            }

            #endregion

            var destinationDirectory = AuthorAspCustomFolderPath;
            IEnumerable<string> ignoreFiles = null;

            if (toBinary)
            {
                destinationDirectory = AuthorAspBinFolderPath;

                ignoreFiles = xmlConfigManager.Deserialize<string[]>(ListOfVanillaFilesOfWebAuthorAspBinFolderFilePath);
            }

            var inputParameters =
                xmlConfigManager.GetAllInputParamsValues(InputParametersFilePath.AbsolutePath);
            files
                .ToList()
                .ForEach(x =>
                {
                    var sourceFilePath = Path.Combine(PackagesFolderPath, x);

                    if (!fileManager.FileExists(sourceFilePath))
                    {
                        throw new ArgumentException($"InvalidPath for {sourceFilePath} file.");
                    }

                    string destinationFilePath = Path.Combine(destinationDirectory, x);
                    if (ignoreFiles != null && ignoreFiles.Any(y => y == destinationFilePath))
                    {
                        _invoker.AddAction(new WriteWarningAction(Logger,
                            () => (true),
                            $"Skip file {x}, because it present in vanilla version."));
                        return;
                    }

                    string destinatioFolderPath = Path.GetDirectoryName(destinationFilePath);
                    _invoker.AddAction(new DirectoryEnsureExistsAction(Logger, destinatioFolderPath));

                    bool destinationFileExists = fileManager.FileExists(destinationFilePath);

                    _invoker.AddAction(new FileCopyAndReplacePlaceholdersAction(
                        logger, sourceFilePath, destinationFilePath, inputParameters));

                    _invoker.AddAction(new WriteWarningAction(Logger, () => ( destinationFileExists ), $"File {destinationFilePath} has been overritten."));
                });
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}