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
        /// The path to ~\Web\Author\ASP\XSL\TopDocumentButtonbar.xml
        /// </summary>
        public static class TopDocumentButtonBarXml
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\XSL\TopDocumentButtonbar.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(_ishDeployment, ISHPaths.IshDeploymentType.Web,
                @"Author\ASP\XSL\TopDocumentButtonbar.xml");

            /// <summary>
            /// The translation job attribute value
            /// </summary>
            public const string TranslationJobAttribute = "NAME=\"TranslationJob\"";

            /// <summary>
            /// The top document buttonbar xpath
            /// </summary>
            public const string TopDocumentTranslationJobXPath = "BUTTONBAR/BUTTON[INPUT[@NAME = 'TranslationJob']]";
        }
    }
}
