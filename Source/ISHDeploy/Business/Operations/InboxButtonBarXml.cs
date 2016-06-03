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
        /// The path to ~\Web\Author\ASP\XSL\InboxButtonBar.xml
        /// </summary>
        public static class InboxButtonBarXml
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\XSL\InboxButtonBar.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\XSL\InboxButtonBar.xml");

            /// <summary>
            /// The Xopus add check out comment placeholder
            /// </summary>
            public const string XopusAddCheckOut = "XOPUS ADD \"CHECK OUT WITH XOPUS\" START";

            /// <summary>
            /// The Xopus remove checkout download comment placeholder
            /// </summary>
            public const string XopusRemoveCheckoutDownload = "XOPUS REMOVE \"CHECKOUT & DOWNLOAD\" START";

            /// <summary>
            /// The Xopus remove check in comment placeholder
            /// </summary>
            public const string XopusRemoveCheckIn = "XOPUS REMOVE \"CHECK IN\" START";
        }
    }
}
