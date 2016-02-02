using System;
using System.IO;

namespace InfoShare.Deployment.Models
{
    public class ISHProject
    {
        public string InstallPath { get; set; }
        public string AuthorFolderPath => Path.Combine(InstallPath, $"Web{Suffix}");
        public string Suffix { get; set; }
        public Version Version { get; set; }
    }
}
