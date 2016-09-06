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
using System.ServiceModel.Security;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Certificate;
using ISHDeploy.Data.Actions.DataBase;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHAPIWCFService
{
    /// <summary>
    /// Sets WCF service to use a certificate that matches to thumbprint
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHAPIWCFServiceCertificateOperation : BaseOperationPaths, IOperation
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
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <param name="validationMode">The certificate validation mode.</param>
        public SetISHAPIWCFServiceCertificateOperation(ILogger logger, Models.ISHDeployment ishDeployment, string thumbprint, X509CertificateValidationMode validationMode) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting of Thumbprint and issuers values to configuration");

            var normalizedThumbprint = new string(thumbprint.ToCharArray().Where(char.IsLetterOrDigit).ToArray());

            if (normalizedThumbprint.Length != thumbprint.Length)
            {
                logger.WriteWarning($"The thumbprint '{thumbprint}' has been normalized to '{normalizedThumbprint}'");
                thumbprint = normalizedThumbprint;
            }

            var serviceCertificateSubjectName = string.Empty;
            (new GetCertificateSubjectByThumbprintAction(logger, thumbprint, result => serviceCertificateSubjectName = result)).Execute();

            // Ensure DataBase file exists
            _invoker.AddAction(new SqlCompactEnsureDataBaseExistsAction(logger, InfoShareSTSDataBasePath.AbsolutePath, $"{InputParameters.BaseUrl}/{InputParameters.STSWebAppName}"));
            _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareSTSDataBasePath));

            // Update STS database
            var encryptedThumbprint = string.Empty;
            (new GetEncryptedRawDataByThumbprintAction(Logger, thumbprint, result => encryptedThumbprint = result)).Execute();

            _invoker.AddAction(new SqlCompactExecuteAction(Logger,
                InfoShareSTSDataBaseConnectionString,
                string.Format(InfoShareSTSDataBase.UpdateCertificateInRelyingPartiesSQLCommandFormat,
                        encryptedThumbprint,
                        string.Join(", ", InfoShareSTSDataBase.GetSvcPaths(InputParameters.BaseUrl, InputParameters.WebAppNameWS)))));

            // Stop STS Application pool before updating RelyingParties 
            _invoker.AddAction(new StopApplicationPoolAction(logger, InputParameters.STSAppPoolName));

            // thumbprint
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareAuthorWebConfigPath, InfoShareAuthorWebConfig.CertificateReferenceFindValueAttributeXPath, thumbprint));
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfigPath, InfoShareWSWebConfig.CertificateThumbprintXPath, thumbprint));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ServiceCertificateThumbprintXPath, thumbprint));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ServiceCertificateSubjectNameXPath, serviceCertificateSubjectName));

            // validationMode
            _invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfigPath, FeedSDLLiveContentConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfigPath, TranslationOrganizerConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfigPath, SynchronizeToLiveContentConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.InfoShareWSServiceCertificateValidationModeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfigPath, InfoShareWSConnectionConfig.InfoShareWSServiceCertificateValidationModeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ServiceCertificateValidationModeXPath, validationMode.ToString()));


            // Recycling Application pool for STS
            _invoker.AddAction(new RecycleApplicationPoolAction(logger, InputParameters.STSAppPoolName, true));

            // Waiting until files becomes unlocked
            _invoker.AddAction(new FileWaitUnlockAction(logger, InfoShareSTSWebConfigPath));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
            Logger.WriteWarning("This cmdlet modified the cookie encryption. All existing browser and client sessions must be recreated.");
        }
    }
}
