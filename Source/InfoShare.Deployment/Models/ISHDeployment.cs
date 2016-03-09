using System;
using System.Collections.Generic;
using System.IO;

namespace InfoShare.Deployment.Models
{
    public class ISHDeployment
    {
        public ISHDeployment(Dictionary<string, string> parameters, Version softwareVersion)
        {
            OriginalParameters = parameters;
            SoftwareVersion = softwareVersion;
        }
        
        public Dictionary<string, string> OriginalParameters { get; }

        public Version SoftwareVersion { get; }

        public string Name => $"InfoShare{GetSuffix()}";
        
        public string AppPath => OriginalParameters["apppath"];

        public string WebPath => OriginalParameters["webpath"];

        public string DataPath => OriginalParameters["datapath"];

        public string ConnectString => OriginalParameters["connectstring"];

        public string DatabaseType => OriginalParameters["databasetype"];

        public string WebNameCM => Path.Combine(GetAuthorFolderPath(), "Author");

        public string WebNameWS => Path.Combine(GetAuthorFolderPath(), "InfoShareWS");

        public string WebNameSTS => Path.Combine(GetAuthorFolderPath(), "InfoShareSTS");

        public string AccessHostName => OriginalParameters["localservicehostname"];

        public string GetAuthorFolderPath() => Path.Combine(WebPath, $"Web{GetSuffix()}");

        public string GetSuffix() => OriginalParameters["projectsuffix"];
    }
}
