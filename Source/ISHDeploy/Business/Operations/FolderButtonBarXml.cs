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
        /// The path to ~\Web\Author\ASP\XSL\FolderButtonbar.xml
        /// </summary>
        public static class FolderButtonBarXml
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\XSL\FolderButtonbar.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\XSL\FolderButtonbar.xml");

            ///// <summary>
            ///// The xpath of "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer" element in file ~\Web\Author\ASP\XSL\FolderButtonbar.xml
            ///// </summary>
            //public const string WSTrustEndpointUrlXPath = "configuration/trisoft.utilities.serviceReferences/serviceUser/issuer";

            /// <summary>
            /// The Xopus add check out comment placeholder
            /// </summary>
            public const string XopusAddCheckOut = "XOPUS ADD \"CHECK OUT WITH XOPUS\" START";

            /// <summary>
            /// The Xopus add undo check out comment placeholder
            /// </summary>
            public const string XopusAddUndoCheckOut = "XOPUS ADD \"UNDO CHECK OUT\" START";

        }
    }
}
