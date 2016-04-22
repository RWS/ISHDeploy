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
        /// The instance of the deployment.
        /// </summary>
        private static Models.ISHDeployment _ishDeployment;

        /// <summary>
        /// Initialization of a class to build the full paths to files
        /// </summary>
        /// <param name="ishDeployment">Instance of the current <see cref="ISHDeployment"/>.</param>
        public static void Initialize(Models.ISHDeployment ishDeployment)
        {
            _ishDeployment = ishDeployment;
        }
    }
}
