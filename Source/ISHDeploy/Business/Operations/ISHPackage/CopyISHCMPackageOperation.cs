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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.ISHUIElement;
using ISHDeploy.Data.Managers.Interfaces;
﻿using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    public class CopyISHCMPackageOperation : BaseOperationPaths
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Path to zip file
        /// </summary>
        private readonly string _zipFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyISHCMPackageOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="zipFilePath">Path to zip file.</param>
        public CopyISHCMPackageOperation(ILogger logger, Models.ISHDeployment ishDeployment, string zipFilePath) :
            base(logger, ishDeployment)
        {
            _zipFilePath = zipFilePath;

            string temporaryDirectory = @"f:\tempfolder";

            var fileManager = ObjectFactory.GetInstance<IFileManager>();

            _invoker = new ActionInvoker(logger, "Copying package to environment");

            // Unzip package
            fileManager.ExtractPackageToDirectory(zipFilePath, temporaryDirectory);

            // Get files list with paths
            var filesList = fileManager.GetFiles(temporaryDirectory, "*.*", true);

            // Separate _config.xml file ans any *Buttonbar.xml from other files
            var configFile = filesList.SingleOrDefault(x => !x.ToLower().Contains("_config.xml"));
            var buttonbarFiles = filesList.Where(x => !x.ToLower().Contains("buttonbar.xml"));

            // if _config.xml exist do update _config.xml
            if (configFile != null)
            {
                // Add action to update _config.xml file

                filesList.Remove(configFile);
            }

            // For each *Buttonbar.xml do update appropriate *Buttonbar.xml
            foreach (var buttonbarFile in buttonbarFiles)
            {
                // Get actualPath 
                string actualPath = buttonbarFile; //Path.Combine(ishDeployment.) buttonbarFile
                if (fileManager.FileExists(actualPath))
                {
                    // Get list of ButtonBarItem models
                    IEnumerable<ButtonBarItem> buttonBarItems = new List<ButtonBarItem>();

                    foreach (var item in buttonBarItems)
                    {
                        _invoker.AddAction(new SetUIElementAction(Logger, new ISHFilePath(AuthorFolderPath, BackupWebFolderPath, item.RelativeFilePath), item));
                    }

                    filesList.Remove(buttonbarFile);
                }

            }

            // For other files do copy with overwrite 
            foreach (var otherFile in filesList)
            {
                // Add copy with replace action
                //_invoker.AddAction(new );
            }



            _invoker.AddAction(new DirectoryEnsureExistsAction(logger, PackagesFolderPath));
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
