namespace InfoShare.Deployment.Data
{
    public static class CommentPatterns
    {
        public const string XopusAddCheckOut = "XOPUS ADD \"CHECK OUT WITH XOPUS\"";
        public const string XopusAddUndoCheckOut = "XOPUS ADD \"UNDO CHECK OUT\"";
        public const string XopusRemoveCheckoutDownload = "XOPUS REMOVE \"CHECKOUT & DOWNLOAD\"";
        public const string XopusRemoveCheckIn = "XOPUS REMOVE \"CHECK IN\"";
        public const string XopusRemoveUndoCheckOut = "XOPUS REMOVE \"UNDO CHECK OUT\"";

        #region ISHExternalPreview

        public const string TrisoftExternalPreviewModule = "<add name=\"TrisoftExternalPreviewModule\"";
        public const string SectionTrisoftInfoshareWebExternalPreviewModule = "<section name=\"trisoft.infoshare.web.externalpreviewmodule\"";
        public const string IdentityExternalId = "<identity externalId=\"THE_FISHEXTERNALID_TO_USE\"";

        #endregion
    }
}
