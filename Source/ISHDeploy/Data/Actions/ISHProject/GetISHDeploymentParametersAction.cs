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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.ISHProject
{
    /// <summary>
    /// Gets parameters for given deployment on current system.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class GetISHDeploymentParametersAction : BaseActionWithResult<IEnumerable<ISHDeploymentParameter>>
    {
        private const string password = "password";
        private const string hiddenPassword = "*******";

        private bool _showPassword;
        private bool _changed;
        private bool _original;
        private string _originalFilePath;
        private string _changedFilePath;

        /// <summary>
        /// The XML configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;
        
        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetISHDeploymentParametersAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="originalFilePath">Full path to original file.</param>
        /// <param name="changedFilePath">Full path to cahnged file.</param>
        /// <param name="original">Get initial data for deployment.</param>
        /// <param name="changed">Get difference from initial data for deployment.</param>
        /// <param name="showPassword">Show real passwords.</param>
        /// <param name="returnResult">The delegate that returns collection with parameters.</param>
        public GetISHDeploymentParametersAction(ILogger logger,
                                                string originalFilePath,
                                                string changedFilePath,
                                                bool original,
                                                bool changed,
                                                bool showPassword,
                                                Action<IEnumerable<ISHDeploymentParameter>> returnResult) : base(logger, returnResult)
        {
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _showPassword = showPassword;
            _changed = changed;
            _original = original;
            _originalFilePath = originalFilePath;
            _changedFilePath = changedFilePath;
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns>Collection with parameters depend on flags.</returns>
        protected override IEnumerable<ISHDeploymentParameter> ExecuteWithResult()
        {
            Dictionary<string, string> dictionary;

            // If we want data from original backup file
            if (_original && _fileManager.FileExists(_originalFilePath))
            {
                dictionary = _xmlConfigManager.GetAllInputParamsValues(_originalFilePath);
            }
            else
            {
                dictionary = _xmlConfigManager.GetAllInputParamsValues(_changedFilePath);

                // To show only difference
                if (_changed)
                {
                    // if backup file does not exist return empty data
                    if (!_fileManager.FileExists(_originalFilePath))
                    {
                        dictionary = new Dictionary<string, string>();
                    }
                    else
                    {
                        dictionary = dictionary
                            .Except(_xmlConfigManager.GetAllInputParamsValues(_originalFilePath))
                            .ToDictionary(t => t.Key, t => t.Value);
                    }
                }
            }

            // To hide passwords
            if (!_showPassword)
            {
                var hiddenDictionary = new Dictionary<string, string>();
                foreach (var element in dictionary)
                {
                    if (element.Key.Contains(password))
                    {
                        hiddenDictionary.Add(element.Key, hiddenPassword);
                    }
                    else
                    {
                        hiddenDictionary.Add(element.Key, element.Value);
                    }
                }
                dictionary = hiddenDictionary;
            }

            return from t in dictionary
                   select new ISHDeploymentParameter {
                       Name = t.Key, 
                       Value= t.Value
                   };
        }
    }
}
