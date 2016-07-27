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
    public class SetISHAPIWCFServiceCertificateOperation : BasePathsOperation, IOperation
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
            _invoker.AddAction(new SqlCompactEnsureDataBaseExistsAction(logger, Deployment.InfoShareSTSDataBasePath.AbsolutePath, $"{Deployment.InputParameters.BaseUrl}/{Deployment.InputParameters.STSWebAppName}"));
            _invoker.AddAction(new FileWaitUnlockAction(logger, Deployment.InfoShareSTSDataBasePath));

            // Update STS database
            var encryptedThumbprint = string.Empty;
            (new GetEncryptedRawDataByThumbprintAction(Logger, thumbprint, result => encryptedThumbprint = result)).Execute();

            _invoker.AddAction(new SqlCompactExecuteAction(Logger,
                Deployment.InfoShareSTSDataBaseConnectionString,
                string.Format(InfoShareSTSDataBase.UpdateCertificateSQLCommandFormat,
                        encryptedThumbprint,
                        string.Join(", ", InfoShareSTSDataBase.GetSvcPaths(Deployment.InputParameters.BaseUrl, Deployment.InputParameters.WebAppNameWS)))));

            // Stop STS Application pool before updating RelyingParties 
            _invoker.AddAction(new StopApplicationPoolAction(logger, Deployment.InputParameters.STSAppPoolName));
            
            // thumbprint
            _invoker.AddAction(new SetAttributeValueAction(logger, Deployment.InfoShareAuthorWebConfigPath, InfoShareAuthorWebConfig.CertificateReferenceFindValueAttributeXPath, thumbprint));
            _invoker.AddAction(new SetAttributeValueAction(logger, Deployment.InfoShareSTSConfigPath, InfoShareSTSConfig.CertificateThumbprintAttributeXPath, thumbprint));
            _invoker.AddAction(new SetAttributeValueAction(logger, Deployment.InfoShareWSWebConfigPath, InfoShareWSWebConfig.CertificateThumbprintXPath, thumbprint));
            _invoker.AddAction(new SetElementValueAction(Logger, Deployment.InputParametersFilePath, InputParameters.ServiceCertificateThumbprintXPath, thumbprint));
            _invoker.AddAction(new SetElementValueAction(Logger, Deployment.InputParametersFilePath, InputParameters.ServiceCertificateSubjectNameXPath, serviceCertificateSubjectName));

            // validationMode
            _invoker.AddAction(new SetAttributeValueAction(logger, Deployment.FeedSDLLiveContentConfigPath, FeedSDLLiveContentConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, Deployment.TranslationOrganizerConfigPath, TranslationOrganizerConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, Deployment.SynchronizeToLiveContentConfigPath, SynchronizeToLiveContentConfig.InfoShareWSServiceCertificateValidationModeAttributeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, Deployment.TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.InfoShareWSServiceCertificateValidationModeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, Deployment.InfoShareWSConnectionConfigPath, InfoShareWSConnectionConfig.InfoShareWSServiceCertificateValidationModeXPath, validationMode.ToString()));
            _invoker.AddAction(new SetElementValueAction(Logger, Deployment.InputParametersFilePath, InputParameters.ServiceCertificateValidationModeXPath, validationMode.ToString()));


            // Recycling Application pool for STS
            _invoker.AddAction(new RecycleApplicationPoolAction(logger, Deployment.InputParameters.STSAppPoolName, true));

            // Waiting until files becomes unlocked
            _invoker.AddAction(new FileWaitUnlockAction(logger, Deployment.InfoShareSTSWebConfigPath));
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
