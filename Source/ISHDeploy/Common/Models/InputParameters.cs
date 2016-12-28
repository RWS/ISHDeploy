using System.Collections.Generic;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    /// Represents the all parameters from inputparameter.xml file
    /// </summary>
    public class InputParameters
    {
        /// <summary>
        /// The HTTPS prefix
        /// </summary>
        private const string HttpsPrefix = "https://";

        /// <summary>
        /// Trisoft Application Pool Prefix
        /// </summary>
        private const string TrisoftAppPoolPrefix = "TrisoftAppPool";

        /// <summary>
        /// The path to inputparameters.xml
        /// </summary>
        public string FilePath { get; }

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

        /// <summary>
        /// Gets the deployment suffix.
        /// </summary>
        public string ProjectSuffix { get; }

        /// <summary>
        /// Gets the DB connection string.
        /// </summary>
        public string ConnectString { get; }

        /// <summary>
        /// Gets the name of the OS user.
        /// </summary>
        public string OSUser { get; }

        /// <summary>
        /// Gets the name of the CM main url folder.
        /// </summary>
        public string CMWebAppName { get; }

        /// <summary>
        /// Gets the name of the WS main url folder.
        /// </summary>
        public string WSWebAppName { get; }

        /// <summary>
        /// Gets the name of the STS main url folder.
        /// </summary>
        public string STSWebAppName { get; }

        /// <summary>
        /// Gets the local service host name.
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// WS Application pool name
        /// </summary>
        public string WSAppPoolName { get; }

        /// <summary>
        /// STS Application pool name
        /// </summary>
        public string STSAppPoolName { get; }

        /// <summary>
        /// CM Application pool name
        /// </summary>
        public string CMAppPoolName { get; }

        /// <summary>
        /// CM certificate thumbprint
        /// </summary>
        public string ServiceCertificateThumbprint { get; }

        /// <summary>
        /// Issuer certificate thumbprint
        /// </summary>
        public string IssuerCertificateThumbprint { get; }

        /// <summary>
        /// Gets the web site name.
        /// </summary>
        public string WebSiteName { get; }

        /// <summary>
        /// Gets the base host name.
        /// </summary>
        public string BaseHostName { get; }

        /// <summary>
        /// Gets the local service host name.
        /// </summary>
        public string LocalServiceHostName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputParameters"/> class.
        /// </summary>
        /// <param name="filePath">The inputparameters.xml file path.</param>
        /// <param name="parameters">The parameters.</param>
        public InputParameters(string filePath, Dictionary<string, string> parameters)
        {
            FilePath = filePath;
            ProjectSuffix = parameters["projectsuffix"];
            AppPath = parameters["apppath"];
            WebPath = parameters["webpath"];
            DataPath = parameters["datapath"];
            DatabaseType = parameters["databasetype"];
            AccessHostName = parameters["baseurl"].Substring(HttpsPrefix.Length);
            WebAppNameCM = parameters["infoshareauthorwebappname"];
            WebAppNameWS = parameters["infosharewswebappname"];
            WebAppNameSTS = parameters["infosharestswebappname"];
            ConnectString = parameters["connectstring"];
            WebPath = parameters["webpath"];
            OSUser = parameters["osuser"];
            CMWebAppName = parameters["infoshareauthorwebappname"];
            WSWebAppName = parameters["infosharewswebappname"];
            STSWebAppName = parameters["infosharestswebappname"];
            BaseUrl = parameters["baseurl"];
            WSAppPoolName = $"{TrisoftAppPoolPrefix}{WSWebAppName}";
            STSAppPoolName = $"{TrisoftAppPoolPrefix}{STSWebAppName}";
            CMAppPoolName = $"{TrisoftAppPoolPrefix}{CMWebAppName}";
            ServiceCertificateThumbprint = parameters["servicecertificatethumbprint"];
            IssuerCertificateThumbprint = parameters["issuercertificatethumbprint"];
            WebSiteName = parameters["websitename"];
            BaseHostName = parameters["basehostname"];
            LocalServiceHostName = parameters["localservicehostname"];
        }
    }
}
