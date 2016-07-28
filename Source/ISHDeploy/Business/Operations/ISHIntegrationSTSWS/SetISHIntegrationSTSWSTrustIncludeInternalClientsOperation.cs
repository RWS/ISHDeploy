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
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWS
{
    /// <summary>
    /// Sets WSTrust configuration including internal clients.
    /// </summary>
    public class SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

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
            _invoker = new ActionInvoker(logger, "Setting of WSTrust configuration including internal clients");

            // endpoint
            _invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfig.Path, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfig.Path, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfig.Path, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustEndpointUrlAttributeName, endpoint.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustEndpointUrlXPath, endpoint.ToString()));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParameters.Path, InputParameters.IssuerWSTrustEndpointUrlXPath, endpoint.ToString()));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParameters.Path, InputParameters.IssuerWSTrustEndpointUrl_NormalizedXPath, endpoint.ToString()));
            // mexEndpoint
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, InfoShareWSWebConfig.Path, InfoShareWSWebConfig.WSTrustMexEndpointUrlHttpsXPath, InfoShareWSWebConfig.WSTrustMexEndpointAttributeName, mexEndpoint.ToString()));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParameters.Path, InputParameters.IssuerWSTrustMexUrlXPath, mexEndpoint.ToString()));
            // bindingType
            _invoker.AddAction(new SetElementValueAction(logger, InfoShareWSConnectionConfig.Path, InfoShareWSConnectionConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, FeedSDLLiveContentConfig.Path, FeedSDLLiveContentConfig.WSTrustEndpointUrlXPath, FeedSDLLiveContentConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, TranslationOrganizerConfig.Path, TranslationOrganizerConfig.WSTrustEndpointUrlXPath, TranslationOrganizerConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            _invoker.AddAction(new SetAttributeValueAction(logger, SynchronizeToLiveContentConfig.Path, SynchronizeToLiveContentConfig.WSTrustEndpointUrlXPath, SynchronizeToLiveContentConfig.WSTrustBindingTypeAttributeName, bindingType.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustBindingTypeXPath, bindingType.ToString()));
            _invoker.AddAction(new SetElementValueAction(logger, InputParameters.Path, InputParameters.IssuerWSTrustBindingTypeXPath, bindingType.ToString()));
            // actorUsername
            if (actorUsername != null)
            {
                _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustActorUserNameXPath, actorUsername));
                _invoker.AddAction(new SetElementValueAction(logger, InputParameters.Path, InputParameters.IssuerActorUserNameXPath, actorUsername));
            }
            // actorPassword
            if (actorPassword != null)
            {
                _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfig.Path, TrisoftInfoShareClientConfig.WSTrustActorPasswordXPath, actorPassword));
                _invoker.AddAction(new SetElementValueAction(logger, InputParameters.Path, InputParameters.IssuerActorPasswordXPath, actorPassword));
            }
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
