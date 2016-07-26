/**
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
using ISHDeploy.Business.Operations.ISHIntegrationSTSWS;
using ISHDeploy.Data.Actions.Certificate;
using ISHDeploy.Data.Actions.DataBase;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Sets STS token signing certificate and/or changes type of authentication.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHSTSConfigurationOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="thumbprint">The Token signing certificate Thumbprint.</param>
        /// <param name="authenticationType">The authentication type.</param>
        public SetISHSTSConfigurationOperation(ILogger logger, Models.ISHDeployment ishDeployment, string thumbprint, AuthenticationTypes authenticationType) :
            base(logger, ishDeployment)
        {
            CheckPermissions();

            _invoker = new ActionInvoker(logger, "Setting of STS token signing certificate and type of authentication");

            AddActionsToStopSTSApplicationPool();
            AddActionsToSetTokenSigningCertificate(thumbprint);
            AddActionsToSetAuthenticationType(authenticationType);
            AddActionsToStartSTSApplicationPool();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="thumbprint">The Token signing certificate Thumbprint.</param>
        public SetISHSTSConfigurationOperation(ILogger logger, Models.ISHDeployment ishDeployment, string thumbprint) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting of STS token signing certificate");

            AddActionsToStopSTSApplicationPool();
            AddActionsToSetTokenSigningCertificate(thumbprint);
            AddActionsToStartSTSApplicationPool();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="authenticationType">The authentication type.</param>
        public SetISHSTSConfigurationOperation(ILogger logger, Models.ISHDeployment ishDeployment, AuthenticationTypes authenticationType) :
            base(logger, ishDeployment)
        {
            CheckPermissions();

            _invoker = new ActionInvoker(logger, "Setting of STS authentication type");

            AddActionsToStopSTSApplicationPool();
            AddActionsToSetAuthenticationType(authenticationType);
            AddActionsToStartSTSApplicationPool();
        }

        /// <summary>
        /// User Access Control check
        /// </summary>
        /// <exception cref="System.Exception">Administrator role not found. Starting new process with elevated rights.</exception>
        private void CheckPermissions()
        {
            var windowsId = System.Security.Principal.WindowsIdentity.GetCurrent();
            var windowsPrincipal = new System.Security.Principal.WindowsPrincipal(windowsId);
            var adminRole = System.Security.Principal.WindowsBuiltInRole.Administrator;

            if (!windowsPrincipal.IsInRole(adminRole))
            {
                throw new Exception("Administrator role not found. Please start new process with elevated rights.");
            }
        }

        /// <summary>
        /// Adds the stop STS application pool action.
        /// </summary>
        private void AddActionsToStopSTSApplicationPool()
        {
            _invoker.AddAction(new StopApplicationPoolAction(Logger, ISHDeploymentInternal.STSAppPoolName));
        }

        /// <summary>
        /// Adds actions for setting STS token signing certificate.
        /// </summary>
        /// <param name="thumbprint">The Token signing certificate Thumbprint.</param>
        private void AddActionsToSetTokenSigningCertificate(string thumbprint)
        {
            var normalizedThumbprint = new string(thumbprint.ToCharArray().Where(char.IsLetterOrDigit).ToArray());

            if (normalizedThumbprint.Length != thumbprint.Length)
            {
                Logger.WriteWarning($"The thumbprint '{thumbprint}' has been normalized to '{normalizedThumbprint}'");
                thumbprint = normalizedThumbprint;
            }

            _invoker.AddAction(new StopApplicationPoolAction(Logger, ISHDeploymentInternal.STSAppPoolName));
            _invoker.AddAction(new SetAttributeValueAction(Logger, InfoShareSTSConfig.Path, InfoShareSTSConfig.CertificateThumbprintAttributeXPath, thumbprint));

            _invoker.AddAction(new UncommentNodesByInnerPatternAction(Logger, InfoShareSTSWebConfig.Path,
                InfoShareSTSWebConfig.TrustedIssuerBehaviorExtensions));

            var parameters = new List<object> { string.Empty };
            parameters.Add(string.Join(", ", InfoShareSTSDataBase.SvcPaths));

            _invoker.AddAction(new GetEncryptedRawDataByThumbprintAction(Logger, thumbprint, result => parameters[0] = result));

            _invoker.AddAction(new SqlCompactUpdateAction(Logger,
                InfoShareSTSDataBase.ConnectionString,
                InfoShareSTSDataBase.UpdateCertificateSQLCommandFormat,
                parameters));
        }

        /// <summary>
        /// Adds actions for setting STS authentication type.
        /// </summary>
        /// <param name="authenticationType">The authentication type.</param>
        private void AddActionsToSetAuthenticationType(AuthenticationTypes authenticationType)
        {
            string currentEndpoint = string.Empty;
            (new GetValueAction(Logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath,
                result => currentEndpoint = result)).Execute();

            if (authenticationType == AuthenticationTypes.Windows)
            {
                // Enable Windows Authentication for STS web site
                _invoker.AddAction(new WindowsAuthenticationSwitcherAction(Logger, ISHDeploymentInternal.STSWebAppName, true));
                // Disable Forms Authentication for STS web site
                _invoker.AddAction(new SetAttributeValueAction(Logger, InfoShareSTSWebConfig.Path, InfoShareSTSWebConfig.AuthenticationModeAttributeXPath, "Windows"));
                //_invoker.AddAction(new RemoveNodesAction(Logger, InfoShareSTSWebConfig.Path, InfoShareSTSWebConfig.AuthenticationFormsElementXPath));


                // If current endpoint is STS endpoint (deployment uses STS as server of authorization)
                // then change the reference to the "issue/wstrust/mixed/windows" endpoint and binding type to WindowsMixed type
                if (currentEndpoint.Contains($"{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameSTS}"))
                {
                    var windowsEndpoint = currentEndpoint.Replace("issue/wstrust/mixed/username", "issue/wstrust/mixed/windows");

                    AddActionsToChangeEndpointAndBindingTypes(BindingType.WindowsMixed, windowsEndpoint);
                }


                // Assign user permissions
                var applicationPoolUser = $@"IIS AppPool\{ISHDeploymentInternal.STSAppPoolName}";
                string pathToCertificate = string.Empty;
                    (new GetPathToCertificateByThumbprintAction(Logger,
                        ISHDeploymentInternal.ServiceCertificateThumbprint, s => pathToCertificate = s)).Execute();

                _invoker.AddAction(new FileSystemRightsAssignAction(Logger, pathToCertificate, applicationPoolUser, FileSystemRightsAssignAction.FileSystemAccessRights.FullControl));
                _invoker.AddAction(new FileSystemRightsAssignAction(Logger, ISHDeploymentInternal.AppPath, applicationPoolUser, FileSystemRightsAssignAction.FileSystemAccessRights.FullControl));
                if (ISHDeploymentInternal.AppPath != ISHDeploymentInternal.DataPath)
                {
                    _invoker.AddAction(new FileSystemRightsAssignAction(Logger, ISHDeploymentInternal.DataPath, applicationPoolUser, FileSystemRightsAssignAction.FileSystemAccessRights.FullControl));

                }
                if (ISHDeploymentInternal.DataPath != ISHDeploymentInternal.WebPath)
                {
                    _invoker.AddAction(new FileSystemRightsAssignAction(Logger, ISHDeploymentInternal.WebPath, applicationPoolUser, FileSystemRightsAssignAction.FileSystemAccessRights.FullControl));

                }


                // Set ApplicationPoolIdentity identityType for STS application pool
                _invoker.AddAction(new SetIdentityTypeAction(Logger, ISHDeploymentInternal.STSAppPoolName, SetIdentityTypeAction.IdentityTypes.ApplicationPoolIdentity));
            }
            else
            {
                // Disable Windows Authentication for STS web site
                _invoker.AddAction(new WindowsAuthenticationSwitcherAction(Logger, ISHDeploymentInternal.STSWebAppName, false));
                // Enable Forms Authentication for STS web site
                _invoker.AddAction(new SetAttributeValueAction(Logger, InfoShareSTSWebConfig.Path, InfoShareSTSWebConfig.AuthenticationModeAttributeXPath, "Forms"));

                // If current endpoint is STS endpoint (deployment uses STS as server of authorization)
                // then change the reference to the "issue/wstrust/mixed/username" endpoint and binding type to UserNameMixed type
                if (currentEndpoint.Contains($"{ISHDeploymentInternal.BaseUrl}/{ISHDeploymentInternal.WebAppNameSTS}"))
                {
                    var usernameEndpoint = currentEndpoint.Replace("issue/wstrust/mixed/windows", "issue/wstrust/mixed/username");

                    AddActionsToChangeEndpointAndBindingTypes(BindingType.UserNameMixed, usernameEndpoint);
                }

                // Set SpecificUser identityType for STS application pool
                _invoker.AddAction(new SetIdentityTypeAction(Logger, ISHDeploymentInternal.STSAppPoolName, SetIdentityTypeAction.IdentityTypes.SpecificUserIdentity));
            }
            _invoker.AddAction(new SetAttributeValueAction(Logger, InfoShareSTSConfig.Path, InfoShareSTSConfig.AuthenticationTypeAttributeXPath, authenticationType.ToString()));
        }

        /// <summary>
        /// Adds actions to change endpoints and binding types
        /// </summary>
        /// <param name="bindingType">The type of binding.</param>
        /// <param name="endpoint">The endpoint.</param>
        private void AddActionsToChangeEndpointAndBindingTypes(BindingType bindingType, string endpoint)
        {
            string bindingTypeAsString = bindingType.ToString();
            // Change ~\Web\InfoShareWS\connectionconfiguration.xml
            _invoker.AddAction(new SetElementValueAction(Logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustBindingTypeXPath, bindingTypeAsString));
            _invoker.AddAction(new SetElementValueAction(Logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath, endpoint));

            // Change ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            _invoker.AddAction(new SetElementValueAction(Logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustBindingTypeXPath, bindingTypeAsString));
            _invoker.AddAction(new SetElementValueAction(Logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustEndpointUrlXPath, endpoint));

            // Change ~\Data\PublishingService\Tools\FeedSDLLiveContent.ps1.config
            _invoker.AddAction(new SetAttributeValueAction(Logger, FeedSDLLiveContentConfig.Path, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustBindingTypeAttributeName, bindingTypeAsString));
            _invoker.AddAction(new SetAttributeValueAction(Logger, FeedSDLLiveContentConfig.Path, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint));

            // Change ~\App\TranslationOrganizer\Bin\TranslationOrganizer.exe.config
            _invoker.AddAction(new SetAttributeValueAction(Logger, TranslationOrganizerConfig.Path, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustBindingTypeAttributeName, bindingTypeAsString));
            _invoker.AddAction(new SetAttributeValueAction(Logger, TranslationOrganizerConfig.Path, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustEndpointUrlAttributeName, endpoint));

            // Change ~\App\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config
            _invoker.AddAction(new SetAttributeValueAction(Logger, SynchronizeToLiveContentConfig.Path, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustBindingTypeAttributeName, bindingTypeAsString));
            _invoker.AddAction(new SetAttributeValueAction(Logger, SynchronizeToLiveContentConfig.Path, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint));

            // InputParameters.xml
            _invoker.AddAction(new SetElementValueAction(Logger, InputParameters.Path, InputParameters.IssuerWSTrustEndpointUrlXPath, endpoint));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParameters.Path, InputParameters.IssuerWSTrustBindingTypeXPath, bindingType.ToString()));
        }

        /// <summary>
        /// Adds start STS application pool action.
        /// </summary>
        private void AddActionsToStartSTSApplicationPool()
        {
            // Recycling Application pool for STS
            _invoker.AddAction(new RecycleApplicationPoolAction(Logger, ISHDeploymentInternal.STSAppPoolName, true));

            // Waiting until files becomes unlocked
            _invoker.AddAction(new FileWaitUnlockAction(Logger, InfoShareSTSWebConfig.Path));
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
