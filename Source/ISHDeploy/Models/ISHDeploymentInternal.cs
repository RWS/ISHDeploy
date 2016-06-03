using System;
using System.Collections.Generic;
using System.IO;

namespace ISHDeploy.Models
{
	/// <summary>
	///	<para type="description">Represents the installed Content Manager deployment extended.</para>
	/// </summary>
	public class ISHDeploymentInternal :ISHDeployment
    {
        /// <summary>
        /// Trisoft Application Pool Prefix
        /// </summary>
        private const string TrisoftAppPoolPrefix = "TrisoftAppPool";

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHDeploymentInternal"/> class.
        /// </summary>
        /// <param name="parameters">The dictionary with all parameters from inputparameter.xml file.</param>
        /// <param name="softwareVersion">The deployment version.</param>
        public ISHDeploymentInternal(Dictionary<string, string> parameters, Version softwareVersion) : base(parameters, softwareVersion)
        {
            _originalParameters = parameters;
        }

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
    }
}
