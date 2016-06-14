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
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions
{
    /// <summary>
    /// Base class for all actions that operate with xml files.
    /// </summary>
    /// <seealso cref="SingleFileAction" />
    public abstract class SingleXmlFileAction : SingleFileAction
	{
        /// <summary>
        /// The xml configuration manager
        /// </summary>
        protected readonly IXmlConfigManager XmlConfigManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleXmlFileAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishFilePath">Wrapper for file path.</param>
        protected SingleXmlFileAction(ILogger logger, ISHFilePath ishFilePath)
			: base(logger, ishFilePath)
        {
			XmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
		}
	}
}
