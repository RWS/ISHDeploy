using System;

namespace Trisoft.Configuration.Automation.Core.Model
{
    public class ISHProject
    {
        public string InstallPath { get; set; }
        public string AuthorFolderPath => $"{InstallPath}\\Web{Suffix}\\Author";
        public string Suffix { get; set; }
        public Version Version { get; set; }
    }
}
