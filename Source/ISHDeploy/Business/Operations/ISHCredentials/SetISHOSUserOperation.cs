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
using System.DirectoryServices.AccountManagement;
using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.COMPlus;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Actions.TextFile;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.Web.Administration;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHCredentials
{
    /// <summary>
    /// Sets OS user credentials.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHOSUserOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHOSUserOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user name.</param>
        public SetISHOSUserOperation(ILogger logger, Models.ISHDeployment ishDeployment, string userName, string password) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting of new OS credential.");

            var contextDomain = new PrincipalContext(ContextType.Domain);
            var principalDomain = UserPrincipal.FindByIdentity(contextDomain, IdentityType.SamAccountName, userName);
            var contextMachine = new PrincipalContext(ContextType.Machine);
            var principalMachine = UserPrincipal.FindByIdentity(contextMachine, IdentityType.SamAccountName, userName);

            if (principalDomain == null && principalMachine == null)
            {
                throw new Exception($"The {userName} user not found");
            }

            if (principalDomain != null)
            {
                if (principalDomain.GetAuthorizationGroups().All(x => x.Name != "Administrators"))
                {
                    throw new Exception($"Administrator role not found for domain user `{userName}`");
                }
            }
            else if (principalMachine.GetAuthorizationGroups().All(x => x.Name != "Administrators"))
            {
                throw new Exception($"Administrator role not found for local user `{userName}`");
            }

            // Stop Application pools
            _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.WSAppPoolName));
            _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.STSAppPoolName));
            _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.CMAppPoolName));

            // WS
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.userName,
                userName));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.password,
                password));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.loadUserProfile,
                true));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                userName));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                password));

            // STS
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.userName,
                userName));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.password,
                password));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.loadUserProfile,
                true));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                userName));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                password));

            // CM
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.userName,
                userName));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.password,
                password));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.loadUserProfile,
                true));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                userName));

            _invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                password));


            _invoker.AddAction(
                    new TextReplaceAction(Logger,
                    SDLISHADFSv3RPInstallPSFilePath,
                    string.Format(SDLISHADFSv3RPInstallPS.OSUserVariableLinePattern, InputParameters.OSUser),
                    string.Format(SDLISHADFSv3RPInstallPS.OSUserVariableLinePattern, userName)));

            _invoker.AddAction(
                    new TextReplaceAction(Logger,
                    IncParamAspFilePath,
                    string.Format(IncParamAsp.OSUserVariableLinePattern, InputParameters.OSUser),
                    string.Format(IncParamAsp.OSUserVariableLinePattern, userName)));

            _invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.OSUserXPath, userName));

            _invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.OSPasswordXPath, password));

            // Start Application pools
            _invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.WSAppPoolName, true));
            _invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName, true));
            _invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.CMAppPoolName, true));

            // Recycle ISH windows services that are running
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var runningServiceNames = serviceManager.GetServicesNamesWithStatus(
                    ishDeployment.Name, 
                    ISHWindowsServiceStatus.Running,
                    InputParameters.ProjectSuffix).ToList();

            // Stop services that are running
            foreach (var service in runningServiceNames)
            {
                _invoker.AddAction(new StopWindowsServiceAction(Logger, service));
            }

            // Set new credentials for all services
            foreach (var serviceName in serviceManager.GetServicesNames(
                    ishDeployment.Name,
                    InputParameters.ProjectSuffix))
            {
                _invoker.AddAction(new SetWindowsServiceCredentialsAction(Logger, serviceName, userName, password));
            }

            // Run services that should be run
            foreach (var service in runningServiceNames)
            {
                _invoker.AddAction(new StartWindowsServiceAction(Logger, service));
            }

            // Check if this operation has implications for several Deployments
            IEnumerable<Models.ISHDeployment> ishDeployments = null;
            new GetISHDeploymentsAction(logger, string.Empty, result => ishDeployments = result).Execute();

            _invoker.AddAction(new WriteWarningAction(Logger, () => (ishDeployments.Count() > 1),
                "The setting of credentials for COM+ components has implications across all deployments."));

            // Set new credentials for COM+ component
            _invoker.AddAction(new SetCOMPlusCredentialsAction(Logger, "Trisoft-InfoShare-Author", userName, password));
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
