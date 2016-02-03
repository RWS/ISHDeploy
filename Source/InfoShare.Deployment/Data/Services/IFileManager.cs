using System.Xml.Linq;

namespace InfoShare.Deployment.Data.Services
{
    public interface IFileManager
    {
        void Backup(string originalFilePath, string backupFilePath);
        void Copy(string sourceFilePath, string destFilePath, bool overwrite = false);
        void CopyToDirectory(string sourceFilePath, string destDir, bool overwrite = false);
        void RestoreOriginal(string backupFilePath, string originalFilePath);
        XDocument Load(string filePath);
        void Save(string filePath, XDocument doc);
    }
}
