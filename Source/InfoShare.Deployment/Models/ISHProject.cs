using System;
using System.Collections.Generic;
using System.IO;

namespace InfoShare.Deployment.Models
{
    public class ISHProject
    {
        public ISHProject(Dictionary<string, string> parameters, Version version)
        {
            InstallParams = parameters;
            Version = version;
        }
        
        public Dictionary<string, string> InstallParams { get; }

        public string AppPath => InstallParams["apppath"];

        public string WebPath => InstallParams["webpath"];

        public string DataPath => InstallParams["datapath"];

        public string AuthorFolderPath => Path.Combine(WebPath, $"Web{Suffix}");

        public string Suffix => InstallParams["projectsuffix"];

        public Version Version { get; }
    }
}
