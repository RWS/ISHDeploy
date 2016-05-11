using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used.
    /// Also provides xpaths to XML elements and attributes in these files.
    /// </summary>
    public partial class OperationPaths
    {
        /// <summary>
        /// The path to ~\Web\InfoShareSTS\Configuration\infoShareSTS.config.
        /// </summary>
        public static class InfoShareSTSConfig
        {
            /// <summary>
            /// The path to ~\Web\InfoShareSTS\Configuration\infoShareSTS.config.
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
                @"InfoShareSTS\Configuration\infoShareSTS.config");
        }
    }
    
}
