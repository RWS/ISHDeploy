namespace InfoShare.Deployment.Data
{
    public static class CommentPatterns
    {
        #region Enable/Disable Content Editor (XOPUS)

        public const string XopusAddCheckOut = "XOPUS ADD \"CHECK OUT WITH XOPUS\"";
        public const string XopusAddUndoCheckOut = "XOPUS ADD \"UNDO CHECK OUT\"";
        public const string XopusRemoveCheckoutDownload = "XOPUS REMOVE \"CHECKOUT & DOWNLOAD\"";
        public const string XopusRemoveCheckIn = "XOPUS REMOVE \"CHECK IN\"";
        public const string XopusRemoveUndoCheckOut = "XOPUS REMOVE \"UNDO CHECK OUT\"";

        #endregion

        #region Enable/Disable Enrich

        public const string EnrichIntegration = "Bluelion integration";

        #endregion

        #region Enable/Disable ExternalPreview

        public const string TrisoftExternalPreviewModuleSearchPattern = "<add name=\"TrisoftExternalPreviewModule\"";
        public const string SectionTrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<section name=\"trisoft.infoshare.web.externalpreviewmodule\"";
        public const string TrisoftInfoshareWebExternalPreviewModuleSearchPattern = "<trisoft.infoshare.web.externalpreviewmodule>";

        public const string TrisoftExternalPreviewModuleXPath = "configuration/system.webServer/modules/add[@name='TrisoftExternalPreviewModule']";
        public const string SectionTrisoftInfoshareWebExternalPreviewModuleXPath = "configuration/configSections/section[@name='trisoft.infoshare.web.externalpreviewmodule']";
        public const string TrisoftInfoshareWebExternalPreviewModuleXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule";

        public const string TrisoftInfoshareWebExternalXPath =
            "configuration/trisoft.infoshare.web.externalpreviewmodule/identity";

        public const string TrisoftInfoshareWebExternalAttributeName = "externalId";

        #endregion
    }
}
