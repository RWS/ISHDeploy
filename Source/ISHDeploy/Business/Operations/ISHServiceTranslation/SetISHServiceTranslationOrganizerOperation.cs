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
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHServiceTranslation
{
    /// <summary>
    /// Sets translation organizer windows service.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHServiceTranslationOrganizerOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceTranslationOrganizerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="parameters">The parameters.</param>
        public SetISHServiceTranslationOrganizerOperation(ILogger logger, Models.ISHDeployment ishDeployment, Dictionary<TranslationOrganizerSetting, object> parameters) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting of translation organizer windows service");

            foreach (var parameter in parameters)
            {
                _invoker.AddAction(
                    new SetAttributeValueAction(Logger, 
                    TranslationOrganizerConfigFilePath,
                    string.Format(TranslationOrganizerConfig.TranslationOrganizerSettingsAttributeXPathPattern, parameter.Key), 
                    HandleStringBeforeSaving(parameter.Key, parameter.Value)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceTranslationOrganizerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="amount">The number of TranslationOrganizer services in the system.</param>
        public SetISHServiceTranslationOrganizerOperation(ILogger logger, Models.ISHDeployment ishDeployment, int amount) :
            base(logger, ishDeployment)
        {
            if (amount > 10)
            {
                throw new Exception($"The {amount} argument is greater than the maximum allowed range of 10.Supply an argument that is less than or equal to 10 and then try the command again");
            }

            _invoker = new ActionInvoker(logger, "Setting of amount of translation organizer windows services");

            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationOrganizer).ToList();

            if (services.Count() > amount)
            {
                // Remove extra services
                var servicesForDeleting = services.Where(serv => serv.Sequence > amount);
                foreach (var service in servicesForDeleting)
                {
                    _invoker.AddAction(new StopWindowsServiceAction(Logger, service));
                    _invoker.AddAction(new RemoveWindowsServiceAction(Logger, service));
                }
            }
            else if (services.Count() < amount)
            {

                var service = services.FirstOrDefault(serv => serv.Sequence == services.Count());
                for (int i = services.Count(); i < amount; i++)
                { 
                    _invoker.AddAction(new CloneWindowsServiceAction(Logger, service, i + 1, InputParameters.OSUser, InputParameters.OSPassword));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHServiceTranslationOrganizerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="infoShareWSUri">The URI to ISHWS.</param>
        /// <param name="wsTrustBindingType">The type of ISHWS authentication.</param>
        /// <param name="wsTrustEndpoint">The URL to issuer ISHWS endpoint.</param>
        /// <param name="infoShareWSServiceCertificateValidationMode">The validation mode.</param>
        /// <param name="infoShareWSDnsIdentity">The DNS Endpoint Identity for ISHWS.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public SetISHServiceTranslationOrganizerOperation(ILogger logger, Models.ISHDeployment ishDeployment, Uri infoShareWSUri, string wsTrustBindingType, Uri wsTrustEndpoint, string infoShareWSServiceCertificateValidationMode = null, string infoShareWSDnsIdentity = null, string userName = null, string password = null) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger,
                $"Setting of ISHWS URL, type of issuer binding, issuer endpoint{(string.IsNullOrEmpty(password) ? "." : " and new credential.")}");

            _invoker.AddAction(
                    new SetAttributeValueAction(Logger,
                    TranslationOrganizerConfigFilePath,
                    TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSAttributeXPath,
                    infoShareWSUri.AbsoluteUri));

            _invoker.AddAction(
                    new SetAttributeValueAction(Logger,
                    TranslationOrganizerConfigFilePath,
                    TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSServiceIssuerWSTrustBindingTypeAttributeXPath,
                    wsTrustBindingType));

            _invoker.AddAction(
                    new SetAttributeValueAction(Logger,
                    TranslationOrganizerConfigFilePath,
                    TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSServiceIssuerWSTrustEndpointAttributeXPath,
                    wsTrustEndpoint.AbsoluteUri));

            if (!string.IsNullOrEmpty(infoShareWSServiceCertificateValidationMode))
            {
                _invoker.AddAction(
                        new SetAttributeValueAction(Logger,
                        TranslationOrganizerConfigFilePath,
                        TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSServiceCertificateValidationModeAttributeXPath,
                        infoShareWSServiceCertificateValidationMode));
            }

            if (!string.IsNullOrEmpty(infoShareWSDnsIdentity))
            {
                _invoker.AddAction(
                        new SetAttributeValueAction(Logger,
                        TranslationOrganizerConfigFilePath,
                        TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSDnsIdentityAttributeXPath,
                        infoShareWSDnsIdentity));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                _invoker.AddAction(
                        new SetAttributeValueAction(Logger,
                        TranslationOrganizerConfigFilePath,
                        TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSServiceUsernameAttributeXPath,
                        userName));
            }

            if (!string.IsNullOrEmpty(password))
            {
                _invoker.AddAction(
                        new SetAttributeValueAction(Logger,
                        TranslationOrganizerConfigFilePath,
                        TranslationOrganizerConfig.TranslationOrganizerSettingsInfoShareWSServicePasswordAttributeXPath,
                        password));
            }
        }

        /// <summary>
        /// Returns value in appropriate string format
        /// </summary>
        /// <param name="type">Type of setting</param>
        /// <param name="value">The value</param>
        /// <returns></returns>
        private string HandleStringBeforeSaving(TranslationOrganizerSetting type, object value)
        {
            if (type == TranslationOrganizerSetting.jobPollingInterval || 
                type == TranslationOrganizerSetting.systemTaskInterval || 
                type == TranslationOrganizerSetting.pendingJobPollingInterval)
            {
                return ((TimeSpan)value).ToString(@"hh\:mm\:ss\.fff");
            }

            return value.ToString();
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
