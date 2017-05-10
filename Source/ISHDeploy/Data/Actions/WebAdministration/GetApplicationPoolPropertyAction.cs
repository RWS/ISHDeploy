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
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Actions.WebAdministration
{
    /// <summary>
    /// Gets property of application pool
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class GetApplicationPoolPropertyAction : BaseActionWithResult<string>
    {
        /// <summary>
        /// The Application Pool name.
        /// </summary>
        private readonly string _appPoolName;

        /// <summary>
        /// The Application Pool property name.
        /// </summary>
        private readonly ApplicationPoolProperty _propertyName;

        /// <summary>
        /// The web Administration manager
        /// </summary>
        private readonly IWebAdministrationManager _webAdminManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetApplicationPoolPropertyAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="appPoolName">Name of the application pool.</param>
        /// <param name="propertyName">The name of ApplicationPool property.</param>
        /// <param name="returnResult">The delegate that returns value.</param>
        public GetApplicationPoolPropertyAction(ILogger logger, string appPoolName, ApplicationPoolProperty propertyName, Action<string> returnResult)
            : base(logger, returnResult)
        {
            _appPoolName = appPoolName;
            _propertyName = propertyName;

            _webAdminManager = ObjectFactory.GetInstance<IWebAdministrationManager>();
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>The ApplicationPool property.</returns>
        protected override string ExecuteWithResult()
        {
            return _webAdminManager.GetApplicationPoolProperty(_appPoolName, _propertyName);
        }
    }
}
