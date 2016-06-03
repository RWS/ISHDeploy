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
        /// The path to ~\Web\Author\ASP\XSL\EventMonitorMenuBar.xml
        /// </summary>
        public static class EventMonitorMenuBarXml
        {
            /// <summary>
            /// The path to ~\Web\Author\ASP\XSL\EventMonitorMenuBar.xml
            /// </summary>
            public static ISHFilePath Path => new ISHFilePath(ISHDeploymentInternal, ISHFilePath.IshDeploymentType.Web,
                @"Author\ASP\XSL\EventMonitorMenuBar.xml");

            /// <summary>
            /// Event monitor tab menu item XPath
            /// </summary>
            public const string EventMonitorTab = "/menubar/menuitem[@label='{0}']";

            /// <summary>
            /// Event monitor tab menu item comment XPath
            /// </summary>
            public const string EventMonitorPreccedingCommentXPath = "/preceding-sibling::node()[not(self::text())][1][not(local-name())]";

            /// <summary>
            /// The event monitor translation jobs comment placeholder
            /// </summary>
            public const string EventMonitorTranslationJobs = "Translation Jobs ===========";
        }
    }
}
