using System.IO;
using System.IO.Compression;

namespace ISHDeploy.Data.Managers
{
    public class PackageManager
    {
        public void PackFolder(string folderPath)
        {
            ZipFile.CreateFromDirectory(folderPath, Path.GetDirectoryName(folderPath) + "\\file.zip");
        }
    }
}
