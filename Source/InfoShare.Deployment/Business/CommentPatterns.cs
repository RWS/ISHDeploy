namespace InfoShare.Deployment.Business
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

        #endregion

        #region Enable/Disable ExternalPreview

        /// <summary>
        /// The Trisoft external preview module search placeholder
        /// </summary>
        public const string TrisoftExternalPreviewModuleSearchPattern = "<add name=\"TrisoftExternalPreviewModule\"";

        /// <summary>
        /// The section Trisoft InfoShare web external preview module search placeholder
        /// </summary>
        public const string SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<section name=\"trisoft.infoshare.web.externalpreviewmodule\"";

        /// <summary>
        /// The Trisoft InfoShare web external preview module search placeholder
        /// </summary>
        public const string TrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<trisoft.infoshare.web.externalpreviewmodule>";

        /// <summary>
        /// The Trisoft external preview module xpath
        /// </summary>
        public const string TrisoftExternalPreviewModuleXPath = "configuration/system.webServer/modules/add[@name='TrisoftExternalPreviewModule']";

        /// <summary>
        /// The section Trisoft Infoshare web external preview module xpath
        /// </summary>
        public const string SectionTrisoftInfoshareWebExternalPreviewModuleXPath = "configuration/configSections/section[@name='trisoft.infoshare.web.externalpreviewmodule']";

        /// <summary>
        /// The Trisoft InfoShare web external preview module xpath
        /// </summary>
        public const string TrisoftInfoshareWebExternalPreviewModuleXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule";

        /// <summary>
        /// The Trisoft InfoShare web external x path
        /// </summary>
        public const string TrisoftInfoshareWebExternalXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule/identity";

        /// <summary>
        /// The Trisoft InfoShare web external attribute name
        /// </summary>
        public const string TrisoftInfoshareWebExternalAttributeName = "externalId";

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
        /// The translation comment comment placeholder
        /// </summary>
        public const string TranslationComment = "TRANSLATION";

        /// <summary>
        /// The top document buttonbar xpath
        /// </summary>
        public const string TopDocumentButtonbarXPath = "BUTTONBAR/BUTTON";

		#endregion

		#region Event Monitor Tab

		/// <summary>
		/// The Xopus add check out comment placeholder
		/// </summary>
		public const string EventMonitorTab = "/menubar/menuitem[@label='{0}']";

		#endregion
	}
}
