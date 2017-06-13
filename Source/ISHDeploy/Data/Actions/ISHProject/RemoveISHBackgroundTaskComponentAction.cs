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

using System.IO;
using ISHDeploy.Common;
using ISHDeploy.Common.Enums;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Data.Actions.ISHProject
{

    /// <summary>
    /// Removes the BackgroundTask component of given role.
    /// </summary>
    /// <seealso cref="SingleFileAction" />
    public class RemoveISHBackgroundTaskComponentAction : SingleFileAction
    {
        /// <summary>
        /// The data aggregate helper
        /// </summary>
        private IDataAggregateHelper _dataAggregateHelper;

        /// <summary>
        /// The BackgroundTask role
        /// </summary>
        private readonly string _role;

        /// <summary>
        /// Initializes new instance of the <see cref="RemoveISHBackgroundTaskComponentAction"/> to save BackgroundTask component status
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="role">The BackgroundTask role</param>
        public RemoveISHBackgroundTaskComponentAction(ILogger logger, ISHFilePath filePath, string role)
            : base(logger, filePath)
        {
            _dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            _role = role;
        }

        /// <summary>
        /// Verify that back up file for vanilla instance is exists
        /// </summary>
        public override void EnsureVanillaBackUpExists()
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            _dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            if (!fileManager.FileExists(FilePath))
            {
                fileManager.EnsureDirectoryExists(Path.GetDirectoryName(FilePath));
                _dataAggregateHelper.SaveComponents(FilePath, new ISHComponentsCollection(true));
            }
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var componentsCollection = _dataAggregateHelper.ReadComponentsFromFile(FilePath);

            var component = componentsCollection[ISHComponentName.BackgroundTask, _role];
            componentsCollection.Components.Remove(component);
            _dataAggregateHelper.SaveComponents(FilePath, componentsCollection);
        }
    }
}
