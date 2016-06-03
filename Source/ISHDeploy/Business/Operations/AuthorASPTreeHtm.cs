using ISHDeploy.Models;

namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class BasePathsOperation
    {
        /// <summary>
        /// The path to ~\Web\Author\ASP\Tree.htm
        /// </summary>
        public static class AuthorASPTreeHtm
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\Tree.htm
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\Tree.htm");

            /// <summary>
            /// The translation job hack comment placeholder
            /// </summary>
            public const string TranslationJobHack = "//Translation Jobs hack";
        }
    }
}
