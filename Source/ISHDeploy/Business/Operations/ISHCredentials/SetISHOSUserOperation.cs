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
using System.DirectoryServices.AccountManagement;
using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.TextFile;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers.Interfaces;
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

            var context = new PrincipalContext(ContextType.Domain);
            var principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);

            if (principal == null)
            {
                throw new Exception($"The {userName} user not found");
            }

            if (principal.GetAuthorizationGroups().All(x => x.Name != "Administrators"))
            {
                throw new Exception($"Administrator role not found for {userName}");
            }

            // Stop Application pools
            _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.WSAppPoolName));
            _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.STSAppPoolName));
            _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.CMAppPoolName));

            // WS
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.UserName,
                userName));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.Password,
                password));

            // STS
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.UserName,
                userName));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.Password,
                password));

            // CM
            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.UserName,
                userName));

            _invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.Password,
                password));

            // ~\App\Setup\STS\ADFS\Scripts\SDL.ISH-ADFSv3.0-RP-Install.ps1
            _invoker.AddAction(
                    new TextReplaceAction(Logger,
                    SDLISHADFSv3RPInstallPSFilePath,
                    string.Format(SDLISHADFSv3RPInstallPS.OSUserVariableLinePattern, InputParameters.OSUser),
                    string.Format(SDLISHADFSv3RPInstallPS.OSUserVariableLinePattern, userName)));

            // ~\Web\Author\ASP\IncParam.asp
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

            // Stop services that are running
            // TODO: Add support of all other services and components
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var services = serviceManager.GetServices(
                    ishDeployment.Name, 
                    ISHWindowsServiceType.TranslationBuilder, 
                    ISHWindowsServiceType.TranslationOrganizer)
                .Where(x => x.Status == ISHWindowsServiceStatus.Running).ToList();
            foreach (var service in services)
            {
                _invoker.AddAction(new StopWindowsServiceAction(Logger, service));
            }

            // Set new credentials for all services
            foreach (var service in serviceManager.GetServices(
                    ishDeployment.Name,
                    ISHWindowsServiceType.TranslationBuilder,
                    ISHWindowsServiceType.TranslationOrganizer))
            {
                _invoker.AddAction(new SetWindowsServiceCredentialsAction(Logger, service, userName, password));
            }

            // Run services that should be run
            foreach (var service in services)
            {
                _invoker.AddAction(new StartWindowsServiceAction(Logger, service));
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
