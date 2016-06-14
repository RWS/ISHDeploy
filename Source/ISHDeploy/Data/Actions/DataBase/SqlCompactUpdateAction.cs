/**
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
using System;
using System.Collections.Generic;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.DataBase
{
    /// <summary>
	/// Action that run update SQL command.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class SqlCompactUpdateAction : BaseAction, ISQLTransactionAction, IDisposable
    {
        /// <summary>
        /// The SQL command executer.
        /// </summary>
        private readonly ISQLCompactCommandExecuter _sqlCommandExecuter;

        /// <summary>
        /// The parameters of SQL command
        /// </summary>
        private readonly List<object> _parameters;
        /// <summary>
        /// The SQL command text.
        /// </summary>
        private readonly string _commandText;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCompactUpdateAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="commandText">The Transact-SQL statement, table name or stored procedure to execute at the data source.</param>
        /// <param name="parameters">The parameters of SQL command</param>
        public SqlCompactUpdateAction(ILogger logger, string connectionString, string commandText, List<object> parameters) 
			: base(logger)
        {
            _sqlCommandExecuter = new SQLCompactCommandExecuter(logger, connectionString, true);
            _parameters = parameters;
            _commandText = commandText;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _sqlCommandExecuter.ExecuteNonQuery(_commandText, _parameters);
        }


        /// <summary>
        /// Commit the SQL transaction
        /// </summary>
        public void TransactionCommit()
        {
            _sqlCommandExecuter.TransactionCommit();
        }

        /// <summary>
        /// Rollback the SQL transaction
        /// </summary>
        public void TransactionRollback()
        {
            _sqlCommandExecuter.TransactionRollback();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _sqlCommandExecuter.Dispose();
        }
    }
}
