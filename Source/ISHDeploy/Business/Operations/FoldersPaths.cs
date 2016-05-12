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
            /// The path to ~\Web\Author\ASP\Editors\Xopus\license\ folder
            /// </summary>
            public static ISHFilePath LicenceFolderPath => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
                @"Author\ASP\Editors\Xopus\license\");
        }
    }
}
