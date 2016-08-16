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
using System.Net;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Interfaces.Actions;

namespace ISHDeploy.Data.Actions.DataBase
{
    /// <summary>
	/// Action that Checks if database file exists, otherwise runs webrequest to create file automatically.
    /// </summary>
    /// <seealso cref="BaseAction" />
    /// <seealso cref="IRestorableAction" />
    public class SqlCompactEnsureDataBaseExistsAction : BaseAction, IDisposable
    {
        /// <summary>
        /// Path to database File.
        /// </summary>
        private readonly string _dbFilePath;

        /// <summary>
        /// Base Url for webRequest
        /// </summary>
        private readonly string _baseUrl;

        /// <summary>
        /// The file manager
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCompactExecuteAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="dbFilePath">The database file path.</param>
        /// <param name="baseUrl">The base URL.</param>
        public SqlCompactEnsureDataBaseExistsAction(ILogger logger, string dbFilePath, string baseUrl)
            : base(logger)
        {
            _dbFilePath = dbFilePath;
            _baseUrl = baseUrl;

            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            // Check if DataBase file exists
            if (!_fileManager.FileExists(_dbFilePath))
            {
                Logger.WriteDebug($"Database file {_dbFilePath}  does not exist");
                SendWebRequest(_baseUrl);
            }
        }

        private void SendWebRequest(string url)
        {
            Logger.WriteDebug($"Creating of web request to {_baseUrl} to perform database file {_dbFilePath} recreation");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.KeepAlive = false;

            if (!_fileManager.FileExists(_dbFilePath))
            {
                try
                {
                    Logger.WriteDebug($"Sending web request to {_baseUrl}");
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Logger.WriteDebug($"The status of response: {response.StatusCode}");

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            int i = 0;
                            while (!_fileManager.FileExists(_dbFilePath))
                            {
                                System.Threading.Thread.Sleep(100);
                                i++;
                                if (i > 100)
                                {
                                    throw new CorruptedInstallationException("Database file was not created after server was restarted.");
                                }
                            }
                            Logger.WriteDebug($"Database file {_dbFilePath} has been created");
                        }
                    }
                }
                catch (WebException e)
                {
                    throw new Exception($"While checking the existence of the database file {_dbFilePath} and trying to make an webrequest to assignment {_baseUrl} the following error has occurred: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
