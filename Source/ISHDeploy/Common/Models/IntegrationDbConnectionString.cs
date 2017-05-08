using ISHDeploy.Common.Enums;

namespace ISHDeploy.Common.Models
{
    /// <summary>
    /// Represents integration database ConnectionString and Database type
    /// </summary>
    public class IntegrationDbConnectionString
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string RawConnectionString { get; }

        /// <summary>
        /// Gets the type of database.
        /// </summary>
        public DatabaseType Engine { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputParameters"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseType">The type of database.</param>
        public IntegrationDbConnectionString(string connectionString, DatabaseType databaseType)
        {
            RawConnectionString = connectionString;
            Engine = databaseType;
        }
    }
}
