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

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class SetISHCMCUILResourceGroupOperation : BaseOperationPaths
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
        /// <param name="zipFilePath">Path to zip file.</param>
        public SetISHCMCUILResourceGroupOperation(ILogger logger, Models.ISHDeployment ishDeployment, string zipFilePath) :
            base(logger, ishDeployment)
        {

            var fileManager = ObjectFactory.GetInstance<IFileManager>();

            string temporaryDirectory = (BackupFolderPath  + @"\Custom").Replace("\\", "/");

            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                var filesList = fileManager
                    .GetFiles($@"{AuthorFolderPath}\Author\ASP\UI\", "*.*", true)
                    .Select(x => x.Substring(x.IndexOf(@"\UI\") + 4).Replace("\\", "/"));
                archive
                    .Entries
                    .Where(x => !filesList.Any(y => y == x.FullName))
                    .ToList()
                    .ForEach(x=> {
                        if (x.Length != 0)
                        {
                            string fileName = temporaryDirectory + '/' + x;
                            fileManager.CreateDirectory(Path.GetDirectoryName(fileName));  
                            x.ExtractToFile(fileName, true);
                        }});
            }

//.Except(filesList) .Where(x => !zipFilesList.Any(y => y == x.FullName))
            //var xmlManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            /*
            _invoker = new ActionInvoker(logger, "Copying package to environment");

            // Unzip package
            fileManager.ExtractPackageToDirectory(zipFilePath, temporaryDirectory);

            // Get files list with paths
            var filesList = fileManager
                .GetFiles(temporaryDirectory, "*.*", true)
                .Where(x => x.ToLower().Contains($"{ishDeployment.SoftwareVersion.Major}.x"))
                .ToList();

            // Separate _config.xml file ans any *Buttonbar.xml from other files (contains both filename and version)
            var configFile = filesList
                .Where(x => x.ToLower().Contains("_config.xml"))
                .FirstOrDefault();

            // Get list of ButtonBarItem models
            var buttonbarFiles = filesList
                .Where(x => x.ToLower().Contains("buttonbar.xml"))
                .ToList();

            // if _config.xml exist do update _config.xml
            if (configFile != null)
            {
                ResourceGroups resourceGroups = null;

                XmlSerializer ser = new XmlSerializer(typeof(ResourceGroups));
                using (XmlReader reader = XmlReader.Create(configFile))
                {
                    reader.ReadToDescendant("resourceGroups");
                    resourceGroups = (ResourceGroups)ser.Deserialize(reader.ReadSubtree());
                }
                foreach (var resourceGroup in resourceGroups.resourceGroups)
                {
                    resourceGroup.ChangeButtonBarItemProperties(Path.GetFileName(configFile));
                    _invoker.AddAction(new SetUIElementAction(Logger,
                                    new ISHFilePath(AuthorFolderPath, BackupWebFolderPath, resourceGroup.RelativeFilePath), resourceGroup));
                }

                filesList.Remove(configFile);
            }

            // For each *Buttonbar.xml do update appropriate *Buttonbar.xml
            foreach (var buttonbarFile in buttonbarFiles)
            {
                string fileName = Path.GetFileName(buttonbarFile);

                if (fileManager.FileExists($@"{AuthorFolderPath}\Author\ASP\XSL\{fileName}"))
                {
                    ButtonBarItemCollection buttonBarItems =
                        xmlManager.Deserialize<ButtonBarItemCollection>(buttonbarFile);

                    if (buttonBarItems.ButtonBarItemArray != null)
                    {
                        foreach (var item in buttonBarItems.ButtonBarItemArray)
                        {
                            item.ChangeButtonBarItemProperties(fileName);
                            _invoker.AddAction(new SetUIElementAction(Logger,
                                new ISHFilePath(AuthorFolderPath, BackupWebFolderPath, item.RelativeFilePath), item));
                        }
                    }

                    filesList.Remove(buttonbarFile);
                }
            }

            // For other files do copy with overwrite
            foreach (var otherFile in filesList)
            {
                // Add copy with replace action
                string filenameWithPartialPath = Path.GetDirectoryName(otherFile).Substring(otherFile.IndexOf(@"Web\") + 4); //for now for Web directory only.
                var newDestination = new ISHFilePath($@"{AuthorFolderPath}\{filenameWithPartialPath}", BackupWebFolderPath, "");
                _invoker.AddAction(new DirectoryCreateAction(logger, newDestination.AbsolutePath));
                _invoker.AddAction(new FileCopyToDirectoryAction(logger, otherFile, newDestination, true));
            }
            */
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            //_invoker.Invoke();
        }
    }
}
