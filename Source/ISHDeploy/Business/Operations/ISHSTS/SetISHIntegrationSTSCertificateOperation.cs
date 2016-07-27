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
﻿using System;
using System.Linq;
using System.ServiceModel.Security;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;
using ISHDeploy.Models.ISHXmlNodes;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Sets Thumbprint and issuers values to configuration.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIntegrationSTSCertificateOperation : BasePathsOperation, IOperation
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
        /// <param name="issuer">The certificate issuer.</param>
        /// <param name="validationMode">The certificate validation mode.</param>
        public SetISHIntegrationSTSCertificateOperation(ILogger logger, Models.ISHDeploymentInternal ishDeployment, string thumbprint, string issuer, X509CertificateValidationMode validationMode) : 
            base(logger, ishDeployment)
		{
			_invoker = new ActionInvoker(logger, "Setting of Thumbprint and issuers values to configuration");

            var normalizedThumbprint = new string(thumbprint.ToCharArray().Where(char.IsLetterOrDigit).ToArray());

		    if (normalizedThumbprint.Length != thumbprint.Length)
		    {
                logger.WriteWarning($"The thumbprint '{thumbprint}' has been normalized to '{normalizedThumbprint}'");
		        thumbprint = normalizedThumbprint;
		    }

		    var menuItem = new IssuerThumbprintItem()
			{
				Thumbprint = thumbprint,
				Issuer = issuer
			};

			// Author web Config
			_invoker.AddAction(new SetNodeAction(logger, InfoShareAuthorWebConfigPath, 
				String.Format(InfoShareAuthorWebConfig.IdentityTrustedIssuersByNameXPath, menuItem.Issuer), menuItem));

			_invoker.AddAction(new SetAttributeValueAction(logger, InfoShareAuthorWebConfigPath,
				InfoShareAuthorWebConfig.CertificateValidationModeXPath, validationMode.ToString()));

			// WS web Config
			_invoker.AddAction(new SetNodeAction(logger, InfoShareWSWebConfigPath, 
				String.Format(InfoShareWSWebConfig.IdentityTrustedIssuersByNameXPath, menuItem.Issuer), menuItem));

			_invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfigPath,
				InfoShareWSWebConfig.CertificateValidationModeXPath, validationMode.ToString()));

			// STS web Config
			var actAsTrustedIssuerThumbprintItem = new ActAsTrustedIssuerThumbprintItem()
			{
				Thumbprint = thumbprint,
				Issuer = issuer
			};

			_invoker.AddAction(new SetNodeAction(logger, InfoShareSTSWebConfigPath,
				String.Format(InfoShareSTSWebConfig.ServiceBehaviorsTrustedUserByNameXPath, menuItem.Issuer), actAsTrustedIssuerThumbprintItem));

			_invoker.AddAction(new UncommentNodesByInnerPatternAction(logger, InfoShareSTSWebConfigPath,
				InfoShareSTSWebConfig.TrustedIssuerBehaviorExtensions));
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
