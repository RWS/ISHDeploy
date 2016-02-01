using System;

namespace InfoShare.Deployment.Models
{
    public class ISHProject
    {
        public string InstallPath { get; set; }
        public string AuthorFolderPath => $"{ InstallPath}{Suffix}";
        public string Suffix { get; set; }
        public Version Version { get; set; }
    }
}
