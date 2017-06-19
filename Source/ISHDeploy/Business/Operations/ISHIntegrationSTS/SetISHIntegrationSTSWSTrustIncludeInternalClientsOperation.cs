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
using ISHDeploy.Common.Enums;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Common.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTS
{
    /// <summary>
    /// Sets WSTrust configuration including internal clients.
    /// </summary>
    public class SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        public IActionInvoker Invoker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="endpoint">The URL to issuer WSTrust endpoint.</param>
        /// <param name="mexEndpoint">The URL to issuer WSTrust mexEndpoint.</param>
        /// <param name="bindingType">The STS issuer authentication type.</param>
        /// <param name="actorUsername">The STS user.</param>
        /// <param name="actorPassword">The password of STS user.</param>
        public SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation(ILogger logger, Models.ISHDeployment ishDeployment, Uri endpoint, Uri mexEndpoint, BindingType bindingType, string actorUsername = null, string actorPassword = null) :
            base (logger, ishDeployment)
        {
            Invoker = new ActionInvoker(logger, "Setting of WSTrust configuration including internal clients");

            // endpoint
            Invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfigPath, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfigPath, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfigFilePath, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfigPath, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            Invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerWSTrustEndpointUrlXPath, endpoint.ToString()));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerWSTrustEndpointUrl_NormalizedXPath, endpoint.ToString().Replace(InputParameters.BaseHostName, InputParameters.LocalServiceHostName)));
            // mexEndpoint
            Invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfigPath, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfigPath, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpsXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            Invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.IssuerWSTrustMexUrlXPath, mexEndpoint.ToString()));
            // bindingType
            Invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfigPath, InfoShareWSConnectionConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfigPath, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfigFilePath, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            Invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfigPath, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            Invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
            Invoker.AddAction(new SetElementValueAction(logger, InputParametersFilePath, InputParametersXml.IssuerWSTrustBindingTypeXPath, bindingType.ToString()));
            // actorUsername
            if (actorUsername != null)
            {
                Invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustActorUserNameXPath, actorUsername));
                Invoker.AddAction(new SetElementValueAction(logger, InputParametersFilePath, InputParametersXml.IssuerActorUserNameXPath, actorUsername));
            }
            // actorPassword
            if (actorPassword != null)
            {
                Invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustActorPasswordXPath, actorPassword));
                Invoker.AddAction(new SetElementValueAction(logger, InputParametersFilePath, InputParametersXml.IssuerActorPasswordXPath, actorPassword));
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
