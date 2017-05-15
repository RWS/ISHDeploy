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

using ISHDeploy.Business.Invokers;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Data.Actions.XmlFile;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHCredentials
{
    /// <summary>
    /// Sets IssuerActor user credentials.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHIssuerActorUserOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetISHIssuerActorUserOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The user name.</param>
        public SetISHIssuerActorUserOperation(ILogger logger, Models.ISHDeployment ishDeployment, string userName, string password) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Setting of new IssuerActor credential.");

            // TODO: Validate user

            // ~\Web\Author\ASP\Trisoft.InfoShare.Client.config
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustActorUserNameXPath, userName));
            _invoker.AddAction(new SetElementValueAction(logger, TrisoftInfoShareClientConfigPath, TrisoftInfoShareClientConfig.WSTrustActorPasswordXPath, password));

            // InputParameters
            _invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ServiceUserNameXPath, userName));
            _invoker.AddAction(new SetElementValueAction(Logger, InputParametersFilePath, InputParametersXml.ServicePasswordXPath, password));

            // ~\Web\InfoShareSTS\Configuration\infoShareSTS.config
            _invoker.AddAction(new SetAttributeValueAction(Logger, InfoShareSTSConfigPath, InfoShareSTSConfig.ActorUsernameAttributeXPath, userName));



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
