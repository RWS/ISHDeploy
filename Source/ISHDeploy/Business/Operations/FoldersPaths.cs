using System.IO;
using ISHDeploy.Extensions;
using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class OperationPaths
    {
        /// <summary>
        /// The path to ~\Web\Author\ASP\Editors\Xopus\license\
        /// </summary>
        public static class FoldersPaths
        {
            /// <summary>
            /// The path to packages folder location for deployment
            /// </summary>
            public static string PackagesFolderPath => Path.Combine(_ishDeployment.GetDeploymentAppDataFolder(), "Packages");

            /// <summary>
            /// The UNC path to packages folder
            /// </summary>
            public static string PackagesFolderUNCPath => ConvertLocalFolderPathToUNCPath(PackagesFolderPath);

            /// <summary>
            /// The path to back up folder location for deployment
            /// </summary>
            public static string BackupFolderPath => Path.Combine(_ishDeployment.GetDeploymentAppDataFolder(), "Backup");
            
            /// <summary>
            /// The path to ~\Web\Author\ASP\Editors\Xopus\license\ folder
            /// </summary>
            public static ISHFilePath LicenceFolderPath => new ISHFilePath(_ishDeployment, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\Editors\Xopus\license\");
        }
    }
}
