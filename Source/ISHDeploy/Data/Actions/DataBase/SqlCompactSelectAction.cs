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
using System;
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Data.Actions.DataBase.Mapper;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.DataBase
{
    /// <summary>
	/// Action that run select SQL command.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class SqlCompactSelectAction<T> : BaseActionWithResult<IEnumerable<T>>, IDisposable where T : class
    {
        /// <summary>
        /// The SQL command executer.
        /// </summary>
        private readonly ISQLCompactCommandExecuter _sqlCommandExecuter;

        /// <summary>
        /// The SQL command text.
        /// </summary>
        private readonly string _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCompactSelectAction{T}" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="query">The Transact-SQL statement, table name or stored procedure to execute at the data source.</param>
        /// <param name="result">The execution result.</param>
        public SqlCompactSelectAction(ILogger logger, string connectionString, string query, Action<IEnumerable<T>> result)
            : base(logger, result)
        {
            _sqlCommandExecuter = new SQLCompactCommandExecuter(logger, connectionString, true);
            _query = query;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        protected override IEnumerable<T> ExecuteWithResult()
        {
            return _sqlCommandExecuter.ExecuteQuery(_query).Select().Select(DataRowToModelMapper.Map<T>);
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
