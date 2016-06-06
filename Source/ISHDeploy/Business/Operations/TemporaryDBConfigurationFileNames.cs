namespace ISHDeploy.Business.Operations
{
    /// <summary>
    /// Provides absolute paths to all ISH files that are going to be used
    /// Also provides xpaths to XML elements and attributes in these files
    /// </summary>
    public partial class BasePathsOperation
    {
        /// <summary>
        /// Temporary names, used to locating SQL templates script for DataBase integration
        /// </summary>
        protected static class TemporaryDBConfigurationFileNames
        {
            /// <summary>
            /// The template for Account Permissions Grant `SQL` script
            /// </summary>
            public const string GrantComputerAccountPermissionsSQLTemplate = "GrantComputerAccountPermissions.sql";

            /// <summary>
            /// The template for Account Permissions Grant `PS1` script
            /// </summary>
            public const string GrantComputerAccountPermissionsPSTemplate = "GrantComputerAccountPermissions.ps1";
        }
    }
}
