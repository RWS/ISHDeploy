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
using System.Linq;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.DataBase
{
    /// <summary>
	/// Action that run update SQL command.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class SqlCompactInsertUpdateAction : SqlCompactExecuteAction
    {
        /// <summary>
        /// The _table name
        /// </summary>
        private readonly string _tableName;

        /// <summary>
        /// The _fields
        /// </summary>
        private readonly Dictionary<string, object> _fields;

        /// <summary>
        /// The _key
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCompactExecuteAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="key">The key.</param>
        public SqlCompactInsertUpdateAction(ILogger logger, string connectionString, string tableName, string key, Dictionary<string, object> fields) 
			: base(logger, connectionString, "")
        {
            _tableName = tableName;
            _fields = fields;
            _key = key;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            string updateSQLCommand =
                $"UPDATE {_tableName} SET {String.Join(", ", _fields.Select(x => $"{x.Key} = '{x.Value}'"))} WHERE {$"{_key} = '{_fields[_key]}'"}";

            if (SQLCommandExecuter.ExecuteNonQuery(updateSQLCommand) == 0)
            {
                string insertSQLCommand =
                    $"INSERT INTO {_tableName} ({String.Join(", ", _fields.Keys)}) VALUES ({String.Join(", ", _fields.Values.Select(x => $"'{x}'"))})";

                SQLCommandExecuter.ExecuteNonQuery(insertSQLCommand);
            }
        }
    }
}
