using System;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Wrapper around SQL operations
    /// </summary>
    public interface ISQLCompactCommandExecuter : IDisposable
    {
        /// <summary>
        /// Transaction state
        /// </summary>
        bool IsCommitted { get; }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection.
        /// </summary>
        /// <returns>The number of rows affected</returns>
        int ExecuteNonQuery();

        /// <summary>
        /// Rollback the SQL transaction
        /// </summary>
        void TransactionRollback();

        /// <summary>
        /// Commit the SQL transaction
        /// </summary>
        void TransactionCommit();
    }
}
