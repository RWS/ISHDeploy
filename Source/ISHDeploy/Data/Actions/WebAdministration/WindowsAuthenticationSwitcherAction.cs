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
﻿using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.WebAdministration
{
    /// <summary>
    /// Disables or enables the windows authentication.
    /// </summary>
    public class WindowsAuthenticationSwitcherAction : BaseAction
    {
        /// <summary>
        /// The site name.
        /// </summary>
        private readonly string _webSiteName;

        /// <summary>
        /// The site name.
        /// </summary>
        private readonly bool _enable;

        /// <summary>
        /// The web Administration manager
        /// </summary>
        private readonly IWebAdministrationManager _webAdministrationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopApplicationPoolAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <param name="enable">if set to <c>true</c> then enable WindowsAuthentication.</param>
        public WindowsAuthenticationSwitcherAction(ILogger logger, string webSiteName, bool enable)
            : base(logger)
        {
            _webSiteName = webSiteName;
            _enable = enable;

            _webAdministrationManager = ObjectFactory.GetInstance<IWebAdministrationManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            if (_enable)
            {
                _webAdministrationManager.EnableWindowsAuthentication(_webSiteName);
                return;
            }
            _webAdministrationManager.DisableWindowsAuthentication(_webSiteName);
        }
    }
}
