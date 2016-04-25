namespace ISHDeploy.Business
{
    /// <summary>
    /// All comment placeholders or xpaths that used for comment/uncomment sections in configuration files
    /// </summary>
    public static class CommentPatterns
    {
        #region Enable/Disable Content Editor (XOPUS)

        /// <summary>
        /// The Xopus add check out comment placeholder
        /// </summary>
        public const string XopusAddCheckOut = "XOPUS ADD \"CHECK OUT WITH XOPUS\" START";

        /// <summary>
        /// The Xopus add undo check out comment placeholder
        /// </summary>
        public const string XopusAddUndoCheckOut = "XOPUS ADD \"UNDO CHECK OUT\" START";

        /// <summary>
        /// The Xopus remove checkout download comment placeholder
        /// </summary>
        public const string XopusRemoveCheckoutDownload = "XOPUS REMOVE \"CHECKOUT & DOWNLOAD\" START";

        /// <summary>
        /// The Xopus remove check in comment placeholder
        /// </summary>
        public const string XopusRemoveCheckIn = "XOPUS REMOVE \"CHECK IN\" START";

        /// <summary>
        /// The Xopus remove undo check out comment placeholder
        /// </summary>
        public const string XopusRemoveUndoCheckOut = "XOPUS REMOVE \"UNDO CHECK OUT\" START";

        #endregion

        #region Enable/Disable Enrich

        /// <summary>
        /// The Enrich integration bluelion plugin comment placeholder
        /// </summary>
        public const string EnrichIntegration = "../BlueLion-Plugin/Bootstrap/bootstrap.js";

        /// <summary>
        /// The Enrich integration bluelion plugin xpath comment placeholder
        /// </summary>
        public const string EnrichIntegrationXPath = "*/*[local-name()='javascript'][@src='../BlueLion-Plugin/Bootstrap/bootstrap.js']";

        /// <summary>
        /// The Enrich integration bluelion configuration comment placeholder
        /// </summary>
        public const string EnrichIntegrationBluelionConfig = "../BlueLion-Plugin/create-toolbar.xml";

        /// <summary>
        /// The Enrich integration bluelion configuration xpath comment placeholder
        /// </summary>
        public const string EnrichIntegrationBluelionConfigXPath = "*/*[local-name()='import'][@src='../BlueLion-Plugin/create-toolbar.xml']";

        /// <summary>
        /// The XPath to Json mimeMap in enrich bluelion web.config file
        /// </summary>
        public const string EnrichBluelionWebConfigJsonMimeMapXPath = "configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']";

        /// <summary>
        /// The XPath to Json mimeMap in enrich bluelion web.config file
        /// </summary>
        public const string EnrichBluelionWebConfigRemoveJsonMimeMapXmlString = "<remove fileExtension=\".json\"/>";

        #endregion
        
        #region Enable/Disable Translation Job

        /// <summary>
        /// The translation job hack comment placeholder
        /// </summary>
        public const string TranslationJobHack = "//Translation Jobs hack";

        /// <summary>
        /// The event monitor translation jobs comment placeholder
        /// </summary>
        public const string EventMonitorTranslationJobs = "Translation Jobs ===========";

        /// <summary>
        /// The translation job attribute value
        /// </summary>
        public const string TranslationJobAttribute = "NAME=\"TranslationJob\"";

        /// <summary>
        /// The top document buttonbar xpath
        /// </summary>
        public const string TopDocumentTranslationJobXPath = "BUTTONBAR/BUTTON[INPUT[@NAME = 'TranslationJob']]";

        #endregion

		#region Event Monitor Tab

		/// <summary>
		/// Event monitor tab menu item XPath
		/// </summary>
		public const string EventMonitorTab = "/menubar/menuitem[@label='{0}']";

		/// <summary>
		/// Event monitor tab menu item comment XPath
		/// </summary>
		public const string EventMonitorPreccedingCommentXPath = "/preceding-sibling::node()[not(self::text())][1][not(local-name())]";

		/// <summary>
		/// The Xopus add check out comment placeholder
		/// </summary>
		public const string EventActionPath = "EventMonitor/Main/Overview?";

		/// <summary>
		/// The Xopus add check out comment placeholder
		/// </summary>
		public const string EventMonitorTabCommentMarkup = " {0} ================================== ";

		#endregion
    }
}
