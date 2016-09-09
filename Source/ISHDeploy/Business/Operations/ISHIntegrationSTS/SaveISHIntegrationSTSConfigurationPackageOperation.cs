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
using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Certificate;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTS
{
    /// <summary>
    /// Saves current STS integration configuration to zip package.
    /// </summary>
    /// <seealso cref="BaseOperationPaths" />
    /// <seealso cref="IOperation" />
    public class SaveISHIntegrationSTSConfigurationPackageOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveISHIntegrationSTSConfigurationPackageOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="packAdfsInvokeScript">if set to <c>true</c> the add ADFS script invocation into package.</param>
        public SaveISHIntegrationSTSConfigurationPackageOperation(ILogger logger, Models.ISHDeployment ishDeployment, string fileName, bool packAdfsInvokeScript = false) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Saving STS integration configuration");

            var packageFilePath = Path.Combine(PackagesFolderPath, fileName);
            var version = new Version(  ishDeployment.SoftwareVersion.Major,
                                        ishDeployment.SoftwareVersion.Minor,
                                        ishDeployment.SoftwareVersion.Revision);
            var temporaryFolder = Path.Combine(Path.GetTempPath(), $"ISHDeploy{version}");
            _invoker.AddAction(new DirectoryEnsureExistsAction(logger, temporaryFolder));
            var temporaryCertificateFilePath = Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.ISHWSCertificateFileName);

            var stsConfigParams = new Dictionary<string, string>
                    {
                        {"$ishhostname", ishDeployment.AccessHostName},
                        {"$ishcmwebappname", ishDeployment.WebAppNameCM},
                        {"$ishwswebappname", ishDeployment.WebAppNameWS},
                        {"$ishwscertificate", TemporarySTSConfigurationFileNames.ISHWSCertificateFileName},
                        {"$ishwscontent", string.Empty}
                    };


            _invoker.AddAction(new SaveThumbprintAsCertificateAction(logger, temporaryCertificateFilePath, InfoShareWSWebConfigPath.AbsolutePath, InfoShareWSWebConfig.CertificateThumbprintXPath));
            _invoker.AddAction(new FileReadAllTextAction(logger, temporaryCertificateFilePath, result => stsConfigParams["$ishwscontent"] = result));

            _invoker.AddAction(new FileGenerateFromTemplateAction(logger,
                TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName,
                Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName),
                stsConfigParams));

            if (packAdfsInvokeScript)
            {
                _invoker.AddAction(new FileGenerateFromTemplateAction(logger,
                    TemporarySTSConfigurationFileNames.ADFSInvokeTemplate,
                    Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.ADFSInvokeTemplate),
                    new Dictionary<string, string>
                    {
                        {"#!#installtool:BASEHOSTNAME#!#", ishDeployment.AccessHostName},
                        {"#!#installtool:PROJECTSUFFIX#!#", InputParameters.ProjectSuffix},
                        {"#!#installtool:OSUSER#!#", InputParameters.OSUser},
                        {"#!#installtool:INFOSHAREAUTHORWEBAPPNAME#!#", ishDeployment.WebAppNameCM},
                        {"#!#installtool:INFOSHAREWSWEBAPPNAME#!#", ishDeployment.WebAppNameWS}
                    }));
            }

            _invoker.AddAction(new DirectoryCreateZipPackageAction(logger, temporaryFolder, packageFilePath));
            _invoker.AddAction(new DirectoryRemoveAction(logger, temporaryFolder));
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
