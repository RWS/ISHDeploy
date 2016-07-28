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
using System.Collections.Generic;
using System.IO;

namespace ISHDeploy.Models
{
	/// <summary>
	///	<para type="description">Represents the installed Content Manager deployment extended.</para>
	/// </summary>
	public class ISHDeploymentInternal : ISHDeployment
    {
        /// <summary>
        /// Trisoft Application Pool Prefix
        /// </summary>
        private const string TrisoftAppPoolPrefix = "TrisoftAppPool";

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHDeploymentInternal"/> class.
        /// </summary>
        /// <param name="inputParametersFilePath">The inputparameters.xml file path.</param>
        /// <param name="parameters">The dictionary with all parameters from inputparameters.xml file.</param>
        /// <param name="softwareVersion">The deployment version.</param>
        public ISHDeploymentInternal(string inputParametersFilePath, Dictionary<string, string> parameters, Version softwareVersion) : base(parameters, softwareVersion)
        {
            InputParametersFilePath = inputParametersFilePath;
            _originalParameters = parameters;
        }

        /// <summary>
        /// Gets the inputparameters.xml file path.
        /// </summary>
        /// <value>
        /// The inputparameters.xml file path.
        /// </value>
        public string InputParametersFilePath { get; }

	    /// <summary>
        /// Gets the all parameters from inputparameter.xml file.
        /// </summary>
        private Dictionary<string, string> _originalParameters { get; }

        /// <summary>
        /// Gets the deployment suffix.
        /// </summary>
        public string ProjectSuffix => _originalParameters["projectsuffix"];

        /// <summary>
        /// Gets the DB connection string.
        /// </summary>
        public string ConnectString => _originalParameters["connectstring"];

        /// <summary>
        /// Gets the path to the Author folder.
        /// </summary>
        public string WebNameCM => Path.Combine(AuthorFolderPath, "Author");

        /// <summary>
        /// Gets the path to the InfoShareWS folder.
        /// </summary>
        public string WebNameWS => Path.Combine(AuthorFolderPath, "InfoShareWS");

        /// <summary>
        /// Gets the path to the InfoShareSTS folder.
        /// </summary>
        public string WebNameSTS => Path.Combine(AuthorFolderPath, "InfoShareSTS");

        /// <summary>
        /// Gets the path to the InfoShareSTS Application Data folder.
        /// </summary>
        public string WebNameSTSAppData => Path.Combine(WebNameSTS, "App_Data");

        /// <summary>
        /// Gets the path to the Web+Suffix Author folder.
        /// </summary>
        public string AuthorFolderPath => Path.Combine(WebPath, $"Web{ProjectSuffix}");

        /// <summary>
        /// Gets the path to the App+Suffix Author folder.
        /// </summary>
        public string AppFolderPath => Path.Combine(AppPath, $"App{ProjectSuffix}");

        /// <summary>
        /// Gets the path to the Data+Suffix Author folder.
        /// </summary>
        public string DataFolderPath => Path.Combine(AppPath, $"Data{ProjectSuffix}");

        /// <summary>
        /// Gets the name of the OS user.
        /// </summary>
        public string OSUser => _originalParameters["osuser"];
 
        /// <summary>
        /// Gets the name of the CM main url folder.
        /// </summary>
        public string CMWebAppName => _originalParameters["infoshareauthorwebappname"];

        /// <summary>
        /// Gets the name of the WS main url folder.
        /// </summary>
        public string WSWebAppName => _originalParameters["infosharewswebappname"];

        /// <summary>
        /// Gets the name of the STS main url folder.
        /// </summary>
        public string STSWebAppName => _originalParameters["infosharestswebappname"];

        /// <summary>
        /// Gets the local service host name.
        /// </summary>
        public string BaseUrl => _originalParameters["baseurl"];

        /// <summary>
        /// WS Application pool name
        /// </summary>
        public string WSAppPoolName => $"{TrisoftAppPoolPrefix}{WSWebAppName}";

        /// <summary>
        /// STS Application pool name
        /// </summary>
        public string STSAppPoolName => $"{TrisoftAppPoolPrefix}{STSWebAppName}";

	    /// <summary>
	    /// CM Application pool name
	    /// </summary>
	    public string CMAppPoolName => $"{TrisoftAppPoolPrefix}{CMWebAppName}";

        /// <summary>
        /// CM certificate thumbprint
        /// </summary>
        public string ServiceCertificateThumbprint => _originalParameters["servicecertificatethumbprint"];
    }
}
