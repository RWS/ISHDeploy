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

using System.IO;
using ISHDeploy.Common.Enums;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.StringActions;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Common.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTS
{
    /// <summary>
    /// Enable internal STS access
    /// </summary>
    /// <seealso cref="IOperation" />
    public class EnableISHIntegrationSTSInternalAuthenticationOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="lCHost">Host name.</param>
        /// <param name="lCWebAppName">Application name.</param>
        public EnableISHIntegrationSTSInternalAuthenticationOperation(ILogger logger, Models.ISHDeployment ishDeployment, string lCHost, string lCWebAppName) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Enable internal STS access.");

            // first we need to delete directory
            Invoker.AddAction(new DirectoryRemoveAction(Logger, InternalSTSFolderToChange));

            // Files content generation 
            var indexContent = string.Empty;
            (new CreateIndexHTMLAction(Logger,
                InternalSTSLoginUrlCM,
                InternalSTSLoginUrlWSWithNewFolder,
                InternalSTSLoginUrlSTS,
                lCHost,
                lCWebAppName,
                result => indexContent = result)).Execute();

            // Create index.html and copy connectionconfiguration.xml files
            Invoker.AddAction(new FileCreateAction(Logger, Path.Combine(InternalSTSFilelPath.AbsolutePath, InternalSTSLogin.IndexName), indexContent));
            Invoker.AddAction(new FileCopyToDirectoryAction(logger, InternalSTSSourceConnectionConfigurationFile, InternalSTSFilelPath.AbsolutePath, true));

            // Get authenticationType attribute value from Web\InfoShareSTS\Configuration\infoShareSTS.config 
            string authenticationToChange, urlToChange, authenticationType = string.Empty;
            (new GetValueAction(Logger, InfoShareSTSConfigPath, InfoShareSTSConfig.AuthenticationTypeAttributeXPath,
                result => authenticationType = result)).Execute();

            if (authenticationType != AuthenticationType.Windows.ToString())
            {
                authenticationToChange = BindingType.UserNameMixed.ToString();
                urlToChange = InternalSTSLoginUrlSTS + "issue/wstrust/mixed/username";
            }
            else
            {
                authenticationToChange = BindingType.WindowsMixed.ToString(); ;
                urlToChange = InternalSTSLoginUrlSTS + "issue/wstrust/mixed/windows";
            }

            // Change new created connectionconfiguration.xml
            Invoker.AddAction(new SetElementValueAction(Logger, InternalSTSNewConnectionConfigPath, InfoShareWSConnectionConfig.WSTrustBindingTypeXPath, authenticationToChange));
            Invoker.AddAction(new SetElementValueAction(Logger, InternalSTSNewConnectionConfigPath, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath, urlToChange));
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
