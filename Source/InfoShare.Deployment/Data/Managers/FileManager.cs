using System;
using System.IO;
using System.Xml.Linq;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Managers
{
    public class FileManager : IFileManager
    {
        private readonly ILogger _logger;
        public FileManager(ILogger logger)
        {
            _logger = logger;
        }

        public void Backup(string originalFilePath, string backupFilePath)
        {
            //File.Copy(originalFilePath, backupFilePath);
        }

        public void Copy(string sourceFilePath, string destFilePath, bool overwrite = false)
        {
            File.Copy(sourceFilePath, destFilePath, overwrite);
        }

        public void CopyToDirectory(string sourceFilePath, string destDir, bool overwrite = false)
        {
            Copy(sourceFilePath, Path.Combine(destDir, Path.GetFileName(sourceFilePath)), overwrite);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void RestoreOriginal(string backupFilePath, string originalFilePath)
        {
            //if (string.IsNullOrWhiteSpace(backupFilePath))
            //{
            //    _logger.WriteWarning("File was not restored because backup file path is empty");
            //    return;
            //}

            //File.Copy(backupFilePath, originalFilePath);
        }

        public XDocument Load(string filePath)
        {
            return XDocument.Load(filePath);
        }

        public void Save(string filePath, XDocument doc)
        {
            doc.Save(filePath);
        }

        public string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public bool TryToFindLicenseFile(string licenseFolderPath, string hostName, string licenseFileExtension, out string filePath)
        {
            filePath = Path.Combine(licenseFolderPath, string.Concat(hostName, licenseFileExtension));

            if (File.Exists(filePath))
                return true;

            int i = hostName.IndexOf(".", StringComparison.InvariantCulture);
            if (i > 0)
            {
                return TryToFindLicenseFile(licenseFolderPath, hostName.Substring(i + 1), licenseFileExtension, out filePath);
            }
            
            return false;
        }
    }
}
