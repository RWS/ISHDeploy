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
    /// Sets configuration of TMS.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIntegrationTmsOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationTmsOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="tmsConfiguration">The TMS configuration.</param>
        /// <param name="isExternalJobMaxTotalUncompressedSizeBytesSpecified">Is ExternalJobMaxTotalUncompressedSizeBytes specified.</param>
        /// <param name="isRetriesOnTimeoutSpecified">Is RetriesOnTimeout specified.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="exceptionMessage">The error message.</param>
        public SetISHIntegrationTmsOperation(ILogger logger, Common.Models.ISHDeployment ishDeployment, BaseXMLElement tmsConfiguration, bool isExternalJobMaxTotalUncompressedSizeBytesSpecified, bool isRetriesOnTimeoutSpecified, Dictionary<TmsConfigurationSetting, object> parameters, string exceptionMessage) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting configuration of TMS");
            var filePath = new ISHFilePath(AppFolderPath, BackupAppFolderPath, tmsConfiguration.RelativeFilePath);

            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();

            if (xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, tmsConfiguration.XPath) || !xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, TranslationOrganizerConfig.TmsNodeXPath))
            {
                if (xmlConfigManager.DoesSingleNodeExist(filePath.AbsolutePath, tmsConfiguration.XPath) 
                    && (!isExternalJobMaxTotalUncompressedSizeBytesSpecified 
                        || !isRetriesOnTimeoutSpecified))
                {

                    if (!isExternalJobMaxTotalUncompressedSizeBytesSpecified)
                    {
                        int currentExternalJobMaxTotalUncompressedSizeBytes =
                            int.Parse(xmlConfigManager.GetValue(filePath.AbsolutePath,
                                $"{tmsConfiguration.XPath}/@{TmsConfigurationSetting.externalJobMaxTotalUncompressedSizeBytes}"));

                        ((TmsConfigurationSection) tmsConfiguration).ExternalJobMaxTotalUncompressedSizeBytes =
                            currentExternalJobMaxTotalUncompressedSizeBytes;
                    }

                    if (!isRetriesOnTimeoutSpecified)
                    {
                        int currentRetriesOnTimeout =
                            int.Parse(xmlConfigManager.GetValue(filePath.AbsolutePath,
                                $"{tmsConfiguration.XPath}/@{TmsConfigurationSetting.retriesOnTimeout}"));

                        ((TmsConfigurationSection)tmsConfiguration).RetriesOnTimeout =
                            currentRetriesOnTimeout;
                    }
                }

                _invoker.AddAction(new SetElementAction(
                    logger,
                    filePath,
                    tmsConfiguration));

                foreach (var parameter in parameters)
                {
                    _invoker.AddAction(
                        new SetAttributeValueAction(Logger,
                        TranslationOrganizerConfigFilePath,
                        $"{tmsConfiguration.XPath}/@{parameter.Key}",
                        parameter.Value.ToString(),
                        true));
                }
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
