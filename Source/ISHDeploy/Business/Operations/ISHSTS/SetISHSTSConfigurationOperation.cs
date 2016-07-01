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
﻿using System.Collections.Generic;
﻿using System.Linq;
using ISHDeploy.Business.Invokers;
﻿using ISHDeploy.Data.Actions.Certificate;
﻿using ISHDeploy.Data.Actions.DataBase;
﻿using ISHDeploy.Data.Actions.File;
﻿using ISHDeploy.Data.Actions.WebAdministration;
﻿using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHSTS
{
    /// <summary>
    /// Sets STS token signing certificate and/or type of authentication.
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
			_invoker = new ActionInvoker(logger, "Setting of STS token signing certificate and type of authentication");

            AddStopSTSApplicationPoolAction();
            AddSetTokenSigningCertificateActions(thumbprint);
            AddSetAuthenticationTypeActions(authenticationType);
            AddStartSTSApplicationPoolAction();
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

            AddStopSTSApplicationPoolAction();
            AddSetTokenSigningCertificateActions(thumbprint);
            AddStartSTSApplicationPoolAction();
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
            _invoker = new ActionInvoker(logger, "Setting of STS authentication type");

            AddStopSTSApplicationPoolAction();
            AddSetAuthenticationTypeActions(authenticationType);
            AddStartSTSApplicationPoolAction();
        }

        /// <summary>
        /// Adds the stop STS application pool action.
        /// </summary>
        private void AddStopSTSApplicationPoolAction()
        {
            _invoker.AddAction(new StopApplicationPoolAction(Logger, ISHDeploymentInternal.STSAppPoolName));
        }

        /// <summary>
        /// Adds actions for setting STS token signing certificate.
        /// </summary>
        /// <param name="thumbprint">The Token signing certificate Thumbprint.</param>
        private void AddSetTokenSigningCertificateActions(string thumbprint)
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
        private void AddSetAuthenticationTypeActions(AuthenticationTypes authenticationType)
        {
            _invoker.AddAction(new SetAttributeValueAction(Logger, InfoShareSTSConfig.Path, InfoShareSTSConfig.AuthenticationTypeAttributeXPath, authenticationType.ToString()));

            if (authenticationType == AuthenticationTypes.Windows)
            {
                _invoker.AddAction(new EnableWindowsAuthenticationAction(Logger, ISHDeploymentInternal.STSWebAppName));
            }
            else
            {
                _invoker.AddAction(new DisableWindowsAuthenticationAction(Logger, ISHDeploymentInternal.STSWebAppName));
            }
        }

        /// <summary>
        /// Adds start STS application pool action.
        /// </summary>
        private void AddStartSTSApplicationPoolAction()
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
