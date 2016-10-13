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
using System.Xml;
using System.Xml.Serialization;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.ISHUIElement;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Models.UI.CUIFConfig;
using System.IO.Compression;
using System.Collections.Generic;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class ExpandISHCMFileOperation : BaseOperationPaths
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandISHCMFileOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="zipFilePath">Path to zip file.</param>
        /// <param name="toBinary">If ToBinary switched.</param>
        public ExpandISHCMFileOperation(ILogger logger, Models.ISHDeployment ishDeployment, string zipFilePath, bool toBinary = false) :
            base(logger, ishDeployment)
        {

            var fileManager = ObjectFactory.GetInstance<IFileManager>();

            string expandDirectory = toBinary ? ($@"{AuthorFolderPath}\Author\ASP\bin").Replace("\\", "/")
                                                : ($@"{AuthorFolderPath}\Author\ASP\Custom").Replace("\\", "/");

            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                IEnumerable<ZipArchiveEntry> files = archive.Entries;
                if (toBinary)
                {
                    var filesList = fileManager
                    .GetFiles($@"{AuthorFolderPath}\Author\ASP\bin", "*.*", true)
                    .Select(x => x.Substring(x.IndexOf(@"\bin\") + 5).Replace("\\", "/"));

                    files = files.Where(x => !filesList.Any(y => y == x.FullName));
                }

                files
                .ToList()
                .ForEach(x =>
                {
                    if (x.Length != 0)
                    {
                        string fileName = expandDirectory + '/' + x;
                        fileManager.CreateDirectory(Path.GetDirectoryName(fileName));
                        x.ExtractToFile(fileName, true);
                    }
                });
            }
        }
    }
}