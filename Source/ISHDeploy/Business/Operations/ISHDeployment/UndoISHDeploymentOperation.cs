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
using ISHDeploy.Common.Models.Backup;
using ISHDeploy.Data.Actions.Asserts;
using ISHDeploy.Data.Actions.COMPlus;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Actions.Registry;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.WindowsServices;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Business.Operations.ISHComponent;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHDeployment
{
	/// <summary>
	/// Operation to revert changes to Vanilla state
	/// </summary>
    public class UndoISHDeploymentOperation : BaseOperationPaths, IOperation
	{
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Gets or sets a value indicating whether skip recycle or not. For integration test perspective only.
        /// Please, see https://jira.sdl.com/browse/TS-11329
        /// </summary>
        /// <value>
        ///   <c>true if [true] then skip recycle; otherwise do recycle</c>.
        /// </value>
        public static bool SkipRecycle { get; set; } = false;

        /// <summary>
        /// Reverts all changes to the vanilla
        /// </summary>
        /// <param name="logger">Logger object.</param>
        /// <param name="ishDeployment">Deployment instance <see cref="T:ISHDeploy.Models.ISHDeployment"/></param>
        public UndoISHDeploymentOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
		{
            Invoker = new ActionInvoker(logger, "Reverting of changes to Vanilla state");
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();
            var dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            // Make sure the deployment is 'marked' as started, when the UNDO is finished the deployment should be started
            ishDeployment.Status = ISHDeploymentStatus.Started;

            // Get all ISH windows services
            var services = serviceManager.GetAllServices(
                ishDeployment.Name).ToList();

            // For version 12.X.X only
            DeleteExtensionsLoaderFile();

            // Remove redundant files from BIN
            Invoker.AddAction(new DirectoryBinReturnToVanila(
                logger, 
                BackupFolderPath, 
                "vanilla.web.author.asp.bin.xml",
                $@"{WebFolderPath}\Author\ASP\bin"));

            // Disable internal STS login (remove directory) 
            Invoker.AddAction(new DirectoryRemoveAction(Logger, InternalSTSFolderToChange));

            // Check if this operation has implications for several Deployments
            IEnumerable<Models.ISHDeployment> ishDeployments = null;
            new GetISHDeploymentsAction(logger, string.Empty, result => ishDeployments = result).Execute();

            // Stop all ISH components
            var componentsThatShouldBeStarted = new Models.ISHComponentsCollection(true).Components.Where(x => x.IsEnabled).ToArray();
            if (!SkipRecycle)
            {
                var componentNamesThatShouldBeStarted = componentsThatShouldBeStarted.Select(x => x.Name).ToList();

                var expectedStateOfComponents = dataAggregateHelper.GetExpectedStateOfComponents(CurrentISHComponentStatesFilePath.AbsolutePath).Components.Where(x => x.IsEnabled).ToArray();
                var componentsThatShouldBeDisabled = expectedStateOfComponents.Where(x => !componentNamesThatShouldBeStarted.Contains(x.Name)).ToArray();

                // Disable all components that should not be enabled in Vanilla
                IOperation disableOperation = new DisableISHComponentOperation(logger, ishDeployment, componentsThatShouldBeDisabled);
                Invoker.AddActionsRange(disableOperation.Invoker.GetActions());

                // Stop components that are enabled in Vanilla
                IOperation stopOperation = new StopISHComponentOperation(logger, ishDeployment, componentsThatShouldBeStarted);
                Invoker.AddActionsRange(stopOperation.Invoker.GetActions());

                // Cleaning up STS App_Data folder
                Invoker.AddAction(new FileCleanDirectoryAction(logger, WebNameSTSAppData));
            }

            // Disable Windows Authentication for STS web site
            Invoker.AddAction(new WindowsAuthenticationSwitcherAction(Logger, InputParameters.STSWebAppName, false));

            // Restore InputParameters.xml
            Models.InputParameters vanillaInputParameters;
            bool isInputParameterBackupFileExist = false;
            (new FileExistsAction(logger, InputParametersFilePath.VanillaPath, returnResult => isInputParameterBackupFileExist = returnResult)).Execute();
            if (isInputParameterBackupFileExist)
            {
                var dictionary = xmlConfigManager.GetAllInputParamsValues(InputParametersFilePath.VanillaPath);
                vanillaInputParameters = new Models.InputParameters(InputParametersFilePath.VanillaPath, dictionary);
                Invoker.AddAction(new FileCopyAction(logger, InputParametersFilePath.VanillaPath, InputParametersFilePath.AbsolutePath,
                    true));
            }
            else
            {
                var dictionary = xmlConfigManager.GetAllInputParamsValues(InputParametersFilePath.AbsolutePath);
                vanillaInputParameters = new Models.InputParameters(InputParametersFilePath.AbsolutePath, dictionary);
            }

            // Get current UserName and Password before change
            string currentOSUserName = xmlConfigManager.GetValue(InputParametersFilePath.AbsolutePath, InputParametersXml.OSUserXPath);
            string currentOSPassword = xmlConfigManager.GetValue(InputParametersFilePath.AbsolutePath, InputParametersXml.OSPasswordXPath);

            var doRollbackOfOSUserAndOSPassword = currentOSUserName != vanillaInputParameters.OSUser ||
                                                  currentOSPassword != vanillaInputParameters.OSPassword;

            // Rollback of WindowsServices  
            // If VanillaPropertiesOfWindowsServicesFilePath exists recreate ISH windows services
            if (_fileManager.FileExists(VanillaPropertiesOfWindowsServicesFilePath))
		    {
		        foreach (var service in services)
		        {
		            Invoker.AddAction(new RemoveWindowsServiceAction(Logger, service));
		        }

		        var backedUpWindowsServices =
		            _fileManager.ReadObjectFromFile<ISHWindowsServiceBackupCollection>(
		                VanillaPropertiesOfWindowsServicesFilePath);

		        foreach (var service in backedUpWindowsServices.Services)
		        {
                    Invoker.AddAction(new InstallWindowsServiceAction(Logger, service,
                        vanillaInputParameters.OSUser, vanillaInputParameters.OSPassword));
                }
		    }
            // or do rollback of OSUser credentials if it is needed 
            else if (doRollbackOfOSUserAndOSPassword)
            {
                foreach (var service in services)
                {
                    Invoker.AddAction(new SetWindowsServiceCredentialsAction(Logger, service.Name, 
                        vanillaInputParameters.OSUser,
                        vanillaInputParameters.OSUser,
                        vanillaInputParameters.OSPassword,
                        vanillaInputParameters.OSPassword));
                }
            }

            // Rollback registry values
            if (_fileManager.FileExists(VanillaRegistryValuesFilePath))
            {
                var registryValueCollection =
                    _fileManager.ReadObjectFromFile<RegistryValueCollection>(VanillaRegistryValuesFilePath);

                foreach (var registryValue in registryValueCollection.Values)
                {
                    Invoker.AddAction(new SetRegistryValueAction(logger, registryValue));
                }
            }
            
            // Rolling back credentials for COM+ component
            if (!SkipRecycle)
            {
                if (doRollbackOfOSUserAndOSPassword)
                {
                    Invoker.AddAction(new WriteWarningAction(Logger, () => (ishDeployments.Count() > 1),
                        "The rolling back of credentials for COM+ components has implications across all deployments."));
                    
                    // Rolling back credentials for COM+ component
                    Invoker.AddAction(new SetCOMPlusCredentialsAction(Logger, TrisoftInfoShareAuthorComPlusApplicationName,
                        vanillaInputParameters.OSUser, vanillaInputParameters.OSUser, vanillaInputParameters.OSPassword,
                        vanillaInputParameters.OSPassword));
                }
            }

            // Rolling back changes for Web folder
            Invoker.AddAction(new FileCopyDirectoryAction(logger, BackupWebFolderPath, WebFolderPath));

			// Rolling back changes for Data folder
			Invoker.AddAction(new FileCopyDirectoryAction(logger, BackupDataFolderPath, DataFolderPath));
            
			// Rolling back changes for App folder
			Invoker.AddAction(new FileCopyDirectoryAction(logger, BackupAppFolderPath, AppFolderPath));

            // Removing licenses
            Invoker.AddAction(new FileCleanDirectoryAction(logger, LicenceFolderPath.AbsolutePath));

            #region Rolling back OSUser credentials for AppPools

            // WS
            if (doRollbackOfOSUserAndOSPassword)
            {
                Invoker.AddAction(new SetApplicationPoolPropertyAction(
                    Logger,
                    InputParameters.WSAppPoolName,
                    ApplicationPoolProperty.userName,
                    vanillaInputParameters.OSUser));

                Invoker.AddAction(new SetApplicationPoolPropertyAction(
                    Logger,
                    InputParameters.WSAppPoolName,
                    ApplicationPoolProperty.password,
                    vanillaInputParameters.OSPassword));

                Invoker.AddAction(new SetWebConfigurationPropertyAction(
                    Logger,
                    $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                    "system.webServer/security/authentication/anonymousAuthentication",
                    WebConfigurationProperty.userName,
                    vanillaInputParameters.OSUser));

                Invoker.AddAction(new SetWebConfigurationPropertyAction(
                    Logger,
                    $"{InputParameters.WebSiteName}/{InputParameters.WSWebAppName}",
                    "system.webServer/security/authentication/anonymousAuthentication",
                    WebConfigurationProperty.password,
                    vanillaInputParameters.OSPassword));
            }

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.WSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            // STS
            if (doRollbackOfOSUserAndOSPassword)
            {
                Invoker.AddAction(new SetApplicationPoolPropertyAction(
                    Logger,
                    InputParameters.STSAppPoolName,
                    ApplicationPoolProperty.userName,
                    vanillaInputParameters.OSUser));

                Invoker.AddAction(new SetApplicationPoolPropertyAction(
                    Logger,
                    InputParameters.STSAppPoolName,
                    ApplicationPoolProperty.password,
                    vanillaInputParameters.OSPassword));

                Invoker.AddAction(new SetWebConfigurationPropertyAction(
                    Logger,
                    $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                    "system.webServer/security/authentication/anonymousAuthentication",
                    WebConfigurationProperty.userName,
                    vanillaInputParameters.OSUser));

                Invoker.AddAction(new SetWebConfigurationPropertyAction(
                    Logger,
                    $"{InputParameters.WebSiteName}/{InputParameters.STSWebAppName}",
                    "system.webServer/security/authentication/anonymousAuthentication",
                    WebConfigurationProperty.password,
                    vanillaInputParameters.OSPassword));
            }

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.STSAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            // CM
            if (doRollbackOfOSUserAndOSPassword)
            {
                Invoker.AddAction(new SetApplicationPoolPropertyAction(
                    Logger,
                    InputParameters.CMAppPoolName,
                    ApplicationPoolProperty.userName,
                    vanillaInputParameters.OSUser));

                Invoker.AddAction(new SetApplicationPoolPropertyAction(
                    Logger,
                    InputParameters.CMAppPoolName,
                    ApplicationPoolProperty.password,
                    vanillaInputParameters.OSPassword));

                Invoker.AddAction(new SetWebConfigurationPropertyAction(
                    Logger,
                    $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                    "system.webServer/security/authentication/anonymousAuthentication",
                    WebConfigurationProperty.userName,
                    vanillaInputParameters.OSUser));

                Invoker.AddAction(new SetWebConfigurationPropertyAction(
                    Logger,
                    $"{InputParameters.WebSiteName}/{InputParameters.CMWebAppName}",
                    "system.webServer/security/authentication/anonymousAuthentication",
                    WebConfigurationProperty.password,
                    vanillaInputParameters.OSPassword));
            }

            Invoker.AddAction(new SetApplicationPoolPropertyAction(
                Logger,
                InputParameters.CMAppPoolName,
                ApplicationPoolProperty.identityType,
                ProcessModelIdentityType.SpecificUser));

            #endregion

            // Enable and start Application pools and COM+ component
            if (!SkipRecycle)
            {
                // Enable and start components that are enabled in Vanilla
                IOperation enableComponentsOperation = new EnableISHComponentOperation(logger, ishDeployment, componentsThatShouldBeStarted);
                Invoker.AddActionsRange(enableComponentsOperation.Invoker.GetActions());

                // Waiting until files becomes unlocked
                Invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareAuthorWebConfigPath));
                Invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareSTSWebConfigPath));
                Invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareWSWebConfigPath));
            }

			// Removing Backup folder
			Invoker.AddAction(new DirectoryRemoveAction(logger, PathToISHDeploymentProgramDataFolder));

            // Remove Author\ASP\Custom
            Invoker.AddAction(new DirectoryRemoveAction(logger, $@"{WebFolderPath}\Author\ASP\Custom"));
        }

        /// <summary>
        /// Deletes ~\Web\Author\ASP\UI\Helpers\ExtensionsLoader.js file if file exists.
        /// </summary>
        public void DeleteExtensionsLoaderFile()
        {
            if (_fileManager.FileExists(ExtensionsLoaderFilePath.AbsolutePath))
            {
                Logger.WriteDebug("Delete file", ExtensionsLoaderFilePath.RelativePath);

                Invoker.AddAction(new FileDeleteAction(Logger, ExtensionsLoaderFilePath.AbsolutePath));
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
