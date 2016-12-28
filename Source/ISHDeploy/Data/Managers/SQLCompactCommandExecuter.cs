/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Data;
using System.Data.SqlServerCe;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

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
        /// The SQL command.
        /// </summary>
        private SqlCeCommand _command;

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
        /// <param name="useTransaction">Specifies whether that command is executed as a transaction. 'False' by default</param>
        public SQLCompactCommandExecuter(ILogger logger, string connectionString, bool useTransaction = false)
        {
            _logger = logger;
            _connection = new SqlCeConnection(connectionString);
            _useTransaction = useTransaction;
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection.
        /// </summary>
        /// <param name="commandText">The SQL command as text</param>
        /// <returns>The number of rows affected</returns>
        public int ExecuteNonQuery(string commandText)
        {
            _command = _connection.CreateCommand();
            _command.CommandType = CommandType.Text;
            _command.CommandText = commandText;

            _logger.WriteDebug("Execut SQL command", _command.CommandText);

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            if (_useTransaction)
            {
                _transaction = _connection.BeginTransaction();
            }

            int result = _command.ExecuteNonQuery();

            _logger.WriteVerbose($"The SQL command `{_command.CommandText}` has been executed");

            return result;
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection.
        /// </summary>
        /// <param name="sqlQuery">The SQL command as text</param>
        /// <returns>The number of rows affected</returns>
        public DataTable ExecuteQuery(string sqlQuery)
        {
            _command = _connection.CreateCommand();
            _command.CommandType = CommandType.Text;
            _command.CommandText = sqlQuery;

            _logger.WriteDebug("Execut SQL command", _command.CommandText);

            if (_connection.State != ConnectionState.Open)
            { 
                _connection.Open();
            }

            if (_useTransaction)
            {
                _transaction = _connection.BeginTransaction();
            }


            //Create a new DataTable.
            DataTable resultTable = new DataTable();

            using (SqlCeDataReader sqlReader = _command.ExecuteReader())
            {
                //Load DataReader into the DataTable.
                resultTable.Load(sqlReader);
            }

            _logger.WriteVerbose($"The SQL command `{_command.CommandText}` has been executed");

            return resultTable;
        }

        /// <summary>
        /// Rollback the SQL transaction
        /// </summary>
        public void TransactionRollback()
        {
            _logger.WriteDebug("Roll back SQL command", _command.CommandText);
            if (IsCommitted)
            {
                _logger.WriteVerbose($"Cannot rollback command `{_command.CommandText}` because it has been already committed.");
                return;
            }

            _transaction?.Rollback();
            _logger.WriteVerbose($"The command `{_command.CommandText}` has been rolled back");
        }

        /// <summary>
        /// Commit the SQL transaction
        /// </summary>
        public void TransactionCommit()
        {
            _logger.WriteDebug("Commit transaction", _command.CommandText);
            _transaction?.Commit();
            IsCommitted = true;
        }

        /// <summary>
        /// Performs closing connection.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
            _command?.Dispose();
        }
    }
}
