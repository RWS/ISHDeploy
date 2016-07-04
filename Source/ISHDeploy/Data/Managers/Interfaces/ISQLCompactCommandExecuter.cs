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
using System.Data;

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
        /// <param name="commandText">The SQL command as text</param>
        /// <returns>The number of rows affected</returns>
        int ExecuteNonQuery(string commandText);

        /// <summary>
        /// Executes a Transact-SQL statement against the connection.
        /// </summary>
        /// <param name="sqlQuery">The SQL command as text</param>
        /// <returns>
        /// The number of rows affected
        /// </returns>
        DataTable ExecuteQuery(string sqlQuery);

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
