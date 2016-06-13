namespace ISHDeploy.Interfaces.Actions
{
    /// <summary>
    /// Provides ability to commit and restore SQL command actions.
    /// </summary>
    public interface ISQLTransactionAction
    {
        /// <summary>
        /// Commit the SQL transaction
        /// </summary>
        void TransactionCommit();

        /// <summary>
        /// Rollback the SQL transaction
        /// </summary>
        void TransactionRollback();
    }
}
