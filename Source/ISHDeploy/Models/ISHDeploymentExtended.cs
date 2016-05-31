using System;
using System.Collections.Generic;
using System.IO;

namespace ISHDeploy.Models
{
	/// <summary>
	///	<para type="description">Represents the installed Content Manager deployment extended.</para>
	/// </summary>
	public class ISHDeploymentExtended :ISHDeployment
    {
     
        /// <summary>
        /// Initializes a new instance of the <see cref="ISHDeploymentExtended"/> class.
        /// </summary>
        /// <param name="parameters">The dictionary with all parameters from inputparameter.xml file.</param>
        /// <param name="softwareVersion">The deployment version.</param>
        public ISHDeploymentExtended(Dictionary<string, string> parameters, Version softwareVersion) : base(parameters, softwareVersion)
        {
        }

        /// <summary>
        /// Gets the deployment suffix.
        /// </summary>
        public string ProjectSuffix => OriginalParameters["projectsuffix"];

        /// <summary>
        /// Gets the DB connection string.
        /// </summary>
        public string ConnectString => OriginalParameters["connectstring"];

        /// <summary>
        /// Gets the path to the Author folder.
        /// </summary>
        public string WebNameCM => Path.Combine(GetAuthorFolderPath(), "Author");

        /// <summary>
        /// Gets the path to the InfoShareWS folder.
        /// </summary>
        public string WebNameWS => Path.Combine(GetAuthorFolderPath(), "InfoShareWS");

        /// <summary>
        /// Gets the path to the InfoShareSTS folder.
        /// </summary>
        public string WebNameSTS => Path.Combine(GetAuthorFolderPath(), "InfoShareSTS");

        /// <summary>
        /// Gets the path to the InfoShareSTS Application Data folder.
        /// </summary>
        public string WebNameSTSAppData => Path.Combine(WebNameSTS, "App_Data");

        /// <summary>
        /// Gets the path to the Web+Suffix Author folder.
        /// </summary>
        public string GetAuthorFolderPath() => Path.Combine(WebPath, $"Web{ProjectSuffix}");

        /// <summary>
        /// Gets the path to the App+Suffix Author folder.
        /// </summary>
        public string GetAppFolderPath() => Path.Combine(AppPath, $"App{ProjectSuffix}");

        /// <summary>
        /// Gets the path to the Data+Suffix Author folder.
        /// </summary>
        public string GetDataFolderPath() => Path.Combine(AppPath, $"Data{ProjectSuffix}");

        /// <summary>
        /// Gets the name of the OS user.
        /// </summary>
        public string GetOSUser() => OriginalParameters["osuser"];

    }
}
