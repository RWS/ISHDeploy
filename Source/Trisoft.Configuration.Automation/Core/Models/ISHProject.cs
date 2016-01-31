using System;

namespace Trisoft.Configuration.Automation.Core.Models
{
    public class ISHProject
    {
        public string InstallPath { get; set; }
        public string AuthorFolderPath { get; set; }
        public string Suffix { get; set; }
        public Version Version { get; set; }
    }
}
