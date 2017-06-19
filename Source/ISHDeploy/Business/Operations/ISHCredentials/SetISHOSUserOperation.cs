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
        public IActionInvoker Invoker { get; }

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
            Invoker = new ActionInvoker(logger, "Setting of new OS credential.");

            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();

            // Get current UserName and Password before change
            string currentOSUserName = xmlConfigManager.GetValue(InputParametersFilePath.AbsolutePath, InputParametersXml.OSUserXPath);
            string currentOSPassword = xmlConfigManager.GetValue(InputParametersFilePath.AbsolutePath, InputParametersXml.OSPasswordXPath);
            
            // Check if this operation has implications for several Deployments
            IEnumerable<Models.ISHDeployment> ishDeployments = null;
            new GetISHDeploymentsAction(logger, string.Empty, result => ishDeployments = result).Execute();

            Invoker.AddAction(new WriteWarningAction(Logger, () => (ishDeployments.Count() > 1),
                "The setting of credentials for COM+ components has implications across all deployments."));

            // Set new credentials for COM+ component
            Invoker.AddAction(new SetCOMPlusCredentialsAction(Logger, "Trisoft-InfoShare-Author", userName, currentOSUserName, password, currentOSPassword));

            // Stop Application pools
            Invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.WSAppPoolName));
            Invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.STSAppPoolName));
            Invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.CMAppPoolName));

            // WS
            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.userName,
                userName));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.password,
                password));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.loadUserProfile,
                true));

            Invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                userName));

            Invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                password));

            // STS
            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.userName,
                userName));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.password,
                password));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.loadUserProfile,
                true));

            Invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                userName));

            Invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                password));

            // CM
            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.userName,
                userName));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.password,
                password));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.loadUserProfile,
                true));

            Invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.userName,
                userName));

            Invoker.AddAction(new SetWebConfigurationPropertyAction(
                Logger,
                $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                "system.webServer/security/authentication/anonymousAuthentication",
                WebConfigurationProperty.password,
                password));

            // ~\App\Setup\STS\ADFS\Scripts\SDL.ISH-ADFSv3.0-RP-Install.ps1
            Invoker.AddAction(
                    new TextReplaceAction(Logger,
                    SDLISHADFSv3RPInstallPSFilePath,
                    string.Format(SDLISHADFSv3RPInstallPS.OSUserVariableLinePattern, InputParameters.OSUser),
                    string.Format(SDLISHADFSv3RPInstallPS.OSUserVariableLinePattern, userName)));

            // ~\Web\Author\ASP\IncParam.asp
            Invoker.AddAction(
                    new TextReplaceAction(Logger,
                    IncParamAspFilePath,
                    string.Format(IncParamAsp.OSUserVariableLinePattern, InputParameters.OSUser),
                    string.Format(IncParamAsp.OSUserVariableLinePattern, userName)));

            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.OSUserXPath, userName));

            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.OSPasswordXPath, password));

            // Start Application pools
            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.WSAppPoolName, true));
            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName, true));
            Invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.CMAppPoolName, true));

            // Stop services that are running
            // TODO: Add support of all other services and components
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var runningServiceNames = serviceManager.GetServices(
                ishDeployment.Name,
                ISHWindowsServiceType.BackgroundTask,
                ISHWindowsServiceType.Crawler,
                ISHWindowsServiceType.SolrLucene,
                ISHWindowsServiceType.TranslationBuilder,
                ISHWindowsServiceType.TranslationOrganizer).
                Where(service => service.Status == ISHWindowsServiceStatus.Running).ToList();

            // Stop services that are running
            foreach (var service in runningServiceNames)
            {
                Invoker.AddAction(new StopWindowsServiceAction(Logger, service));
            }

            // Set new credentials for all services
            foreach (var service in serviceManager.GetServices(
                ishDeployment.Name,
                ISHWindowsServiceType.BackgroundTask,
                ISHWindowsServiceType.Crawler,
                ISHWindowsServiceType.SolrLucene,
                ISHWindowsServiceType.TranslationBuilder,
                ISHWindowsServiceType.TranslationOrganizer))
            {
                Invoker.AddAction(new SetWindowsServiceCredentialsAction(Logger, service.Name, userName, currentOSUserName, password, currentOSPassword));
            }

            // Run services that should be run
            foreach (var service in runningServiceNames)
            {
                Invoker.AddAction(new StartWindowsServiceAction(Logger, service));
            }
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
