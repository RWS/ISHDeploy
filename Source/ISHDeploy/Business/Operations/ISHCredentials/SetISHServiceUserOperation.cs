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
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHCredentials
{
    /// <summary>
    /// Sets ServiceUser user credentials.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHServiceUserOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceUserOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user name.</param>
        public SetISHServiceUserOperation(ILogger logger, Models.ISHDeployment ishDeployment, string userName, string password) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Setting of new ServiceUser credential.");

            // FeedSDLLiveContentConfig
            Invoker.AddAction(new SetAttributeValueAction(logger, 
                FeedSDLLiveContentConfigPath, 
                FeedSDLLiveContentConfig.ServiceUserUserNameAttributeXPath, 
                userName));
            Invoker.AddAction(new SetAttributeValueAction(logger, 
                FeedSDLLiveContentConfigPath, 
                FeedSDLLiveContentConfig.ServiceUserPasswordAttributeXPath, 
                password));

            // SynchronizeToLiveContentConfig
            Invoker.AddAction(new SetAttributeValueAction(logger,
                SynchronizeToLiveContentConfigPath,
                SynchronizeToLiveContentConfig.ServiceUserUserNameAttributeXPath,
                userName));
            Invoker.AddAction(new SetAttributeValueAction(logger,
                SynchronizeToLiveContentConfigPath,
                SynchronizeToLiveContentConfig.ServiceUserPasswordAttributeXPath,
                password));


            // InputParameters
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ServiceUserNameXPath, userName));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ServicePasswordXPath, password));

            // Set new ServiceUser credentials for TranslationBuilderConfig service
            Invoker.AddAction(new SetAttributeValueAction(logger,
                TranslationBuilderConfigFilePath,
                TranslationBuilderConfig.AttributeXPaths[TranslationBuilderSetting.userName],
                userName));
            Invoker.AddAction(new SetAttributeValueAction(logger,
                TranslationBuilderConfigFilePath,
                TranslationBuilderConfig.AttributeXPaths[TranslationBuilderSetting.password],
                password));

            // Set new ServiceUser credentials for TranslationOrganizerConfig service
            Invoker.AddAction(new SetAttributeValueAction(Logger,
                        TranslationOrganizerConfigFilePath,
                        TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSServiceUsernameAttributeXPath,
                        userName));

            Invoker.AddAction(
                    new SetAttributeValueAction(Logger,
                    TranslationOrganizerConfigFilePath,
                    TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSServicePasswordAttributeXPath,
                    password));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            Invoker.Invoke();
        }
    }
}
