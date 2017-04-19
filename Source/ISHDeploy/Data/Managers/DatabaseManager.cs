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
using System.Data.OleDb;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Manager that do all kinds of operations with database.
    /// </summary>
    /// <seealso cref="ITemplateManager" />
    public class DatabaseManager : IDatabaseManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DatabaseManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Test connection to database.
        /// </summary>
        /// <param name="connectionString">The connection string to database.</param>
        /// <returns>True if the connection is available</returns>
        public bool TestConnection(string connectionString)
        {
            try
            {
                _logger.WriteDebug("Try to check database connection");
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open(); // throws if invalid
                    _logger.WriteVerbose("Database connection successfully opened");
                }
                return true;
            }
            catch (Exception)
            {
                _logger.WriteVerbose("Invalid database connection");
                return false;
            }
        }
    }
}
