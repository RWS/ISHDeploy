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

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Replaces placeholders on appropriate values from input parameters
    /// </summary>
    public class FileCopyAndReplacePlaceholdersAction : BaseAction
    {
        /// <summary>
        /// The source file path
        /// </summary>
        private readonly string _sourcePath;

        /// <summary>
        /// The destination file path
        /// </summary>
        private readonly string _destinationPath;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// The dictionary with matches between placeholders and input parameters.
        /// </summary>
        private readonly Dictionary<string, string> _matchesDictionary;

        private const string RegexPlaceHolderPattern = @"#!#(.*?)#!#";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopyAndReplacePlaceholdersAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="matchesDictionary">The dictionary with matches between placeholders and input parameters.</param>
        public FileCopyAndReplacePlaceholdersAction(ILogger logger, string sourcePath, string destinationPath, Dictionary<string, string> matchesDictionary) 
			: base(logger)
        {
            _sourcePath = sourcePath;
            _destinationPath = destinationPath;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
            _matchesDictionary = matchesDictionary;
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        public override void Execute()
        {
            string destinationFolderPath = Path.GetDirectoryName(_destinationPath);
            _fileManager.EnsureDirectoryExists(destinationFolderPath);

            if (_sourcePath.ToLower().EndsWith(".txt") | 
                _sourcePath.ToLower().EndsWith(".bat") |
                _sourcePath.ToLower().EndsWith(".cmd") |
                _sourcePath.ToLower().EndsWith(".vbs") | 
                _sourcePath.ToLower().EndsWith(".wsf") |
                _sourcePath.ToLower().EndsWith(".ini") |
                _sourcePath.ToLower().EndsWith(".asp") | 
                _sourcePath.ToLower().EndsWith(".aspx") |
                _sourcePath.ToLower().EndsWith(".master") |
                _sourcePath.ToLower().EndsWith(".xml") | 
                _sourcePath.ToLower().EndsWith(".xsl") |
                _sourcePath.ToLower().EndsWith(".config") |
                _sourcePath.ToLower().EndsWith(".htm") | 
                _sourcePath.ToLower().EndsWith(".html") |
                _sourcePath.ToLower().EndsWith(".css") |
                _sourcePath.ToLower().EndsWith(".js") | 
                _sourcePath.ToLower().EndsWith(".dtd") |
                _sourcePath.ToLower().EndsWith(".sql") |
                _sourcePath.ToLower().EndsWith(".h") | 
                _sourcePath.ToLower().EndsWith(".p") |
                _sourcePath.ToLower().EndsWith(".par") |
                _sourcePath.ToLower().EndsWith(".properties") | 
                _sourcePath.ToLower().EndsWith(".ps1") |
                _sourcePath.ToLower().EndsWith(".psm1"))
            {

                Logger.WriteDebug("Reading of file", _sourcePath);
                string content = _fileManager.ReadAllText(_sourcePath);

                Match m = Regex.Match(content, RegexPlaceHolderPattern);
                while (m.Success)
                {
                    var placeHolder = m.Value.ToLower();
                    var key = placeHolder.Replace("installtool:", string.Empty).Replace("#!#", string.Empty);
                    if (_matchesDictionary.ContainsKey(key))
                    {
                        var value = _matchesDictionary[key];
                        Logger.WriteDebug("Replace placeholder", m.Value, value);
                        content = content.Replace(m.Value, value);
                        Logger.WriteVerbose($"The placeholder {m.Value} has been replaced on {value}");
                    }
                    else
                    {
                        Logger.WriteWarning($"Input parameter {key} in placeholder {m.Value} is not found.");
                    }

                    m = m.NextMatch();
                }

                Logger.WriteDebug("Write content to file", _destinationPath);
                _fileManager.WriteAllText(_destinationPath, content);

                Logger.WriteVerbose($"The file {_destinationPath} has been saved");
            }
            else
            {
                _fileManager.Copy(_sourcePath, _destinationPath, true);
                Logger.WriteVerbose($"The file {_destinationPath} has been copied without replacing of placeholder");
            }
        }
    }
}
