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
﻿using System;
﻿using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Business.Invokers;
﻿using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Common.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTS
{
    /// <summary>
    /// Sets WSTrust configuration.
    /// </summary>
    public class SetISHIntegrationSTSWSTrustOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationSTSWSTrustOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="endpoint">The URL to issuer WSTrust endpoint.</param>
        /// <param name="mexEndpoint">The URL to issuer WSTrust mexEndpoint.</param>
        /// <param name="bindingType">The STS issuer authentication type.</param>
        public SetISHIntegrationSTSWSTrustOperation(ILogger logger, Models.ISHDeployment ishDeployment, Uri endpoint, Uri mexEndpoint, BindingType bindingType) :
            base(logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Setting of WSTrust configuration");

            // endpoint
            Invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfigPath, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerWSTrustEndpointUrlXPath, endpoint.ToString()));
            // mexEndpoint
            Invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfigPath, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfigPath, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpsXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerWSTrustMexUrlXPath, mexEndpoint.ToString()));
            // bindingType
            Invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfigPath, InfoShareWSConnectionConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerWSTrustBindingTypeXPath, bindingType.ToString()));
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
