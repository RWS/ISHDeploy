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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.License
{
    /// <summary>
    /// Checks if license for specific host name exists.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class LicenseTestAction : BaseActionWithResult<bool>
    {
        /// <summary>
        /// Hardcoded value of the license file type
        /// </summary>
		const string LicenseFileExtension = ".txt";

        /// <summary>
        /// The license folder path
        /// </summary>
        private readonly string _licenseFolderPath;

        /// <summary>
        /// The host name
        /// </summary>
        private readonly string _hostname;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseTestAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="licenseFolderPath">The license folder path.</param>
        /// <param name="hostname">The host name for which license is checked.</param>
        /// <param name="returnResult">The delegate that returns if license is found for host name.</param>
        public LicenseTestAction(ILogger logger, ISHFilePath licenseFolderPath, string hostname, Action<bool> returnResult)
            : base(logger, returnResult)
        {
            _licenseFolderPath = licenseFolderPath.AbsolutePath;
            _hostname = hostname;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
		}

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>Result of testin of license file</returns>
        protected override bool ExecuteWithResult()
        {
		    string filePath;

            bool result = _fileManager.TryToFindLicenseFile(_licenseFolderPath, _hostname, LicenseFileExtension, out filePath);

            if (!result)
		    {
                Logger.WriteVerbose($"The license file for host \"{_hostname}\" not found");
            }

            return result;
        }
	}
}
