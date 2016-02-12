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
	}
}
