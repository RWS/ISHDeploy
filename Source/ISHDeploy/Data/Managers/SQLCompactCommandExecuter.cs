using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Wrapper around SQL operations
    /// </summary>
    public class SQLCompactCommandExecuter : ISQLCompactCommandExecuter
    {
        /// <summary>
        /// Transaction state
        /// </summary>
        public bool IsCommitted { get; private set; }

        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The connection used to open the SQL Server database.
        /// </summary>
        private readonly SqlCeConnection _connection;

        /// <summary>
        /// The SQL command text.
        /// </summary>
        private readonly string _commandFormat;

        /// <summary>
        /// The SQL command.
        /// </summary>
        private SqlCeCommand _command;

        /// <summary>
        /// The parameters of SQL command
        /// </summary>
        private readonly List<object> _parameters;

        /// <summary>
        /// Specifies whether that command is executed as a transaction.
        /// </summary>
        private readonly bool _useTransaction;

        /// <summary>
        /// The SQL transaction.
        /// </summary>
        private SqlCeTransaction _transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLCompactCommandExecuter"/> class.
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="connectionString">The connection used to open the SQL Server database</param>
        /// <param name="commandFormat">The SQL command as text</param>
        /// <param name="parameters">The parameters of SQL command</param>
        /// <param name="useTransaction">Specifies whether that command is executed as a transaction. 'False' by default</param>
        public SQLCompactCommandExecuter(ILogger logger, string connectionString, string commandFormat, List<object> parameters, bool useTransaction = false)
        {
            _logger = logger;
            _connection = new SqlCeConnection(connectionString);
            _commandFormat = commandFormat;
            _parameters = parameters;
            _useTransaction = useTransaction;
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection.
        /// </summary>
        /// <returns>The number of rows affected</returns>
        public int ExecuteNonQuery()
        {
            _command = _connection.CreateCommand();
            _command.CommandType = CommandType.Text;
            _command.CommandText = string.Format(_commandFormat, _parameters.ToArray());
            _connection.Open();
            if (_useTransaction)
            {
                _transaction = _connection.BeginTransaction();
            }

            _logger.WriteDebug($"Executing command {_command.CommandText}.");
            int result = _command.ExecuteNonQuery();

            return result;
        }

        /// <summary>
        /// Rollback the SQL transaction
        /// </summary>
        public void TransactionRollback()
        {
            if (IsCommitted)
            {
                _logger.WriteDebug($"Cannot rollback command because it has been already committed {_command.CommandText}.");
                return;
            }

            _logger.WriteDebug($"Rolling back command {_command.CommandText}.");
            _transaction?.Rollback();
        }

        /// <summary>
        /// Commit the SQL transaction
        /// </summary>
        public void TransactionCommit()
        {
            _logger.WriteDebug($"Commiting transaction with command {_command.CommandText}.");
            _transaction?.Commit();
            IsCommitted = true;
        }

        /// <summary>
        /// Performs closing connection.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();

            _connection.Close();
            _connection.Dispose();
            _command.Dispose();
        }
    }
}
