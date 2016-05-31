using System;
using System.Collections.Generic;

namespace ISHDeploy.Models
{
    /// <summary>
    ///	<para type="description">Represents the installed Content Manager deployment.</para>
    /// </summary>
    public class ISHDeployment
    {
        /// <summary>
        /// The HTTPS prefix
        /// </summary>
        private const string HttpsPrefix = "https://";

        /// <summary>
        /// Initializes a new instance of the <see cref="ISHDeployment"/> class.
        /// </summary>
        /// <param name="parameters">The dictionary with all parameters from inputparameter.xml file.</param>
        /// <param name="softwareVersion">The deployment version.</param>
        public ISHDeployment(Dictionary<string, string> parameters, Version softwareVersion)
        {
            OriginalParameters = parameters;
            Name = $"InfoShare{parameters["projectsuffix"]}";
            AppPath = parameters["apppath"];
            WebPath = parameters["webpath"];
            DataPath = parameters["datapath"];
            DatabaseType = parameters["databasetype"];
            AccessHostName = parameters["baseurl"].Substring(HttpsPrefix.Length);
            WebAppNameCM = parameters["infoshareauthorwebappname"];
            WebAppNameWS = parameters["infosharewswebappname"];
            WebAppNameSTS = parameters["infosharestswebappname"];
            SoftwareVersion = softwareVersion;
        }

        /// <summary>
        /// Gets the all parameters from inputparameter.xml file.
        /// </summary>
        protected Dictionary<string, string> OriginalParameters { get; }

        /// <summary>
        /// Gets the deployment version.
        /// </summary>
        public Version SoftwareVersion { get; }

        /// <summary>
        /// Gets the deployment suffix in user-friendly format.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the application path.
        /// </summary>
        public string AppPath { get; set; }

        /// <summary>
        /// Gets the web path.
        /// </summary>
        public string WebPath { get; set; }

        /// <summary>
        /// Gets the data path.
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// Gets the DB type.
        /// </summary>
        public string DatabaseType { get; set; }

        /// <summary>
        /// Gets the name of the access host.
        /// </summary>
        public string AccessHostName { get; set; }
        
        /// <summary>
        /// Gets the name of the CM main url folder.
        /// </summary>
        public string WebAppNameCM { get; set; }

        /// <summary>
        /// Gets the name of the WS main url folder.
        /// </summary>
        public string WebAppNameWS { get; set; }

        /// <summary>
        /// Gets the name of the STS main url folder.
        /// </summary>
        public string WebAppNameSTS { get; set; }
    }
}
