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
﻿using ISHDeploy.Business.Invokers;
﻿using ISHDeploy.Common;
﻿using ISHDeploy.Common.Enums;
﻿using ISHDeploy.Data.Actions.WindowsServices;
﻿using ISHDeploy.Data.Managers.Interfaces;
﻿using ISHDeploy.Common.Interfaces;
using Models = ISHDeploy.Common.Models;

namespace ISHDeploy.Business.Operations.ISHServiceTranslation
{
    /// <summary>
    /// Disables translation organizer windows service.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class DisableISHServiceTranslationOrganizerOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableISHServiceTranslationOrganizerOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        public DisableISHServiceTranslationOrganizerOperation(ILogger logger, Models.ISHDeployment ishDeployment) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Disabling of translation organizer windows service");

            var serviceManager = ObjectFactory.GetInstance<IWindowsServiceManager>();

            var services = serviceManager.GetServices(ishDeployment.Name, ISHWindowsServiceType.TranslationOrganizer);

            foreach (var service in services)
            {
                _invoker.AddAction(
                    new StopWindowsServiceAction(Logger, service));
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
