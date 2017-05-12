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

using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.WebAdministration
{
    /// <summary>
    /// Sets web configuration property
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class SetWebConfigurationPropertyAction : BaseAction, IRestorableAction
    {
        /// <summary>
        /// The Application Pool name.
        /// </summary>
        private readonly string _webSiteName;

        /// <summary>
        /// The xPath to get configuration node.
        /// </summary>
        private readonly string _configurationXPath;

        /// <summary>
        /// The Application Pool property value.
        /// </summary>
        private readonly object _value;

        /// <summary>
        /// The web configuration property name.
        /// </summary>
        private readonly WebConfigurationProperty _propertyName;

        /// <summary>
        /// The web Administration manager
        /// </summary>
        private readonly IWebAdministrationManager _webAdminManager;

        /// <summary>
        /// The Application Pool property previous value.
        /// </summary>
        private object _backedUpValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetWebConfigurationPropertyAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="webSiteName">Name of the web site.</param>
        /// <param name="configurationXPath">The xPath to get configuration node.</param>
        /// <param name="propertyName">The name of WebConfiguration property.</param>
        /// <param name="value">The value.</param>
        public SetWebConfigurationPropertyAction(ILogger logger, string webSiteName, string configurationXPath, WebConfigurationProperty propertyName, object value)
            : base(logger)
        {
            _webSiteName = webSiteName;
            _configurationXPath = configurationXPath;
            _propertyName = propertyName;
            _value = value;

            _webAdminManager = ObjectFactory.GetInstance<IWebAdministrationManager>();
        }

        /// <summary>
        ///	Gets current value before change.
        /// </summary>
        public void Backup()
        {
            _backedUpValue = _webAdminManager.GetWebConfigurationProperty(_webSiteName, _configurationXPath, _propertyName);
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _webAdminManager.SetWebConfigurationProperty(_webSiteName, _configurationXPath, _propertyName, _value);
        }

        /// <summary>
        ///	Reverts a value to initial state.
        /// </summary>
        public void Rollback()
        {
            _webAdminManager.SetWebConfigurationProperty(_webSiteName, _configurationXPath, _propertyName, _backedUpValue);
        }
    }
}
