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

using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Common.Models.TranslationOrganizer;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Business.Operations.ISHServiceTranslation
{
    /// <summary>
    /// Sets configuration of FileSystem.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHTranslationFileSystemExportOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHTranslationFileSystemExportOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="fileSystemConfiguration">The FileSystem configuration.</param>
        /// <param name="isExternalJobMaxTotalUncompressedSizeBytesSpecified">Is ExternalJobMaxTotalUncompressedSizeBytes specified.</param>
        /// <param name="exceptionMessage">The error message.</param>
        public SetISHTranslationFileSystemExportOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, BaseXMLElement fileSystemConfiguration, bool isExternalJobMaxTotalUncompressedSizeBytesSpecified, string exceptionMessage) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting configuration of FileSystem");
            var filePath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, fileSystemConfiguration.RelativeFilePath);

            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();

            if (xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, fileSystemConfiguration.XPath) || !xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, TranslationOrganizerConfig.FileSystemNodeXPath))
            {
                if (xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, fileSystemConfiguration.XPath)
                    && !isExternalJobMaxTotalUncompressedSizeBytesSpecified)
                {
                    if (!isExternalJobMaxTotalUncompressedSizeBytesSpecified)
                    {
                        int currentExternalJobMaxTotalUncompressedSizeBytes =
                            int.Parse(xmlConfigManager.GetValue(filePath.AbsolutePath,
                                $"{fileSystemConfiguration.XPath}/@{FileSystemConfigurationSetting.externalJobMaxTotalUncompressedSizeBytes}"));

                        ((FileSystemConfigurationSection) fileSystemConfiguration)
                            .ExternalJobMaxTotalUncompressedSizeBytes =
                            currentExternalJobMaxTotalUncompressedSizeBytes;
                    }
                }

                _invoker.AddAction(new SetElementAction(
                    logger,
                    filePath,
                    fileSystemConfiguration));
            }
            else
            {
                throw new DocumentAlreadyContainsElementException(exceptionMessage);
            }
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
