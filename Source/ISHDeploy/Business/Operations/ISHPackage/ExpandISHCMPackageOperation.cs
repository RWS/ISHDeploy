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
using System.IO.Compression;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class ExpandISHCMPackageOperation : BaseOperationPaths
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandISHCMPackageOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="zipFilePath">Path to zip file.</param>
        /// <param name="toBinary">If ToBinary switched.</param>
        public ExpandISHCMPackageOperation(ILogger logger, Models.ISHDeployment ishDeployment, string zipFilePath, bool toBinary = false) :
            base(logger, ishDeployment)
        {
            // Validate if file exist
            ValidateFileExist(PackagesFolderPath, zipFilePath);

            var fileManager = ObjectFactory.GetInstance<IFileManager>();

            string destinationDirectory = toBinary ? ($@"{AuthorFolderPath}\Author\ASP\bin")
                                                : ($@"{AuthorFolderPath}\Author\ASP\Custom");

            using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(PackagesFolderPath, zipFilePath)))
            {
                IEnumerable<ZipArchiveEntry> files = archive.Entries;
                files = WorkWithBinaryFolder(toBinary, fileManager, files);

                files
                .ToList()
                .ForEach(x =>
                {
                    if (!string.IsNullOrEmpty(x.Name))
                    {
                        string fileName = destinationDirectory.Replace("\\", "/") + '/' + x; //zip library works with "/" in path
                        bool present = fileManager.FileExists(fileName);
                        fileManager.CreateDirectory(Path.GetDirectoryName(fileName));
                        x.ExtractToFile(fileName, true);
                        if (present)
                            logger.WriteWarning($"File {fileName} was overritten.");
                    }
                });
            }
        }

        private IEnumerable<ZipArchiveEntry> WorkWithBinaryFolder(bool toBinary, IFileManager fileManager, IEnumerable<ZipArchiveEntry> files)
        {
            if (toBinary)
            {
                var fullFileList = Directory.GetFileSystemEntries(
                    $@"{AuthorFolderPath}\Author\ASP\bin", "*.*", SearchOption.AllDirectories);

                string vanilaFile = BackupFolderPath + "\\vanilla.web.author.asp.bin.xml";
                if (!fileManager.FileExists(vanilaFile))
                {
                    fileManager.CreateDirectory(BackupFolderPath);
                    using (var outputFile = File.Create(vanilaFile))
                    {
                        var serializer = new XmlSerializer(typeof(string[]));
                        serializer.Serialize(outputFile, fullFileList);
                    }
                }

                var doc = fileManager.Load(vanilaFile);

                var filesList = doc
                           .Element("ArrayOfString")
                           .Elements("string")
                           .Select(x => x.Value.Substring(x.Value.IndexOf(@"\bin\") + 5).Replace("\\", "/"));//zip library works with "/" in path


                files
                    .Where(x => filesList.Any(y => y == x.FullName))
                    .ToList()
                    .ForEach(x => Logger.WriteWarning($"File {x} skipped, because it present in vanilla version."));

                files = files.Where(x => !filesList.Any(y => y == x.FullName));

            }

            return files;
        }
    }
}