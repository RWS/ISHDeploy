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

using System.Linq;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class CopyISHCMFileOperation : BaseOperationPaths
    {

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

            string destinationDirectory = toBinary ? ($@"{AuthorFolderPath}\Author\ASP\bin")
                                                : ($@"{AuthorFolderPath}\Author\ASP\Custom");

            files = WorkWithBinaryFolder(toBinary, fileManager, files);
            files
                .Select(x => Path.Combine(PackagesFolderPath, x))
                .ToList()
                .ForEach(x => {
                    fileManager.CreateDirectory(destinationDirectory);
                    string newFullFileName = Path.Combine(destinationDirectory,  Path.GetFileName(x));
                    bool present = fileManager.FileExists(newFullFileName);
                    fileManager.CopyToDirectory(x, destinationDirectory, true);
                    if (present)
                        logger.WriteWarning($"File {newFullFileName} was overritten");
                });
        }

        private string[] WorkWithBinaryFolder(bool toBinary, IFileManager fileManager, string[] files)
        {
            if (toBinary)
            {
                var fullFileList = Directory.GetFileSystemEntries(
                    $@"{AuthorFolderPath}\Author\ASP\bin", "*.*", SearchOption.AllDirectories);

                var filesList = fullFileList.Select(x => x.Substring(x.IndexOf(@"\bin\") + 5).Replace("\\", "/"));

                files = files.Except(filesList).ToArray();

                string vanilaFile = BackupFolderPath + "/vanilla.web.author.asp.bin.xml";
                if (!fileManager.FileExists(vanilaFile))
                {
                    fileManager.CreateDirectory(BackupFolderPath);
                    using (var outputFile = File.Create(vanilaFile))
                    {
                        var serializer = new XmlSerializer(typeof(string[]));
                        serializer.Serialize(outputFile, fullFileList);
                    }
                }
            }

            return files;
        }
    }
}