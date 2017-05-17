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
    /// Gets parameters for given deployment on current system.
    /// </summary>
    /// <seealso cref="SingleFileAction" />
    public class SaveISHComponentsAction : SingleFileAction
    {
        /// <summary>
        /// The data aggregate helper
        /// </summary>
        private IDataAggregateHelper _dataAggregateHelper;

        /// <summary>
        /// The collection of InfoShare components
        /// </summary>
        private ISHComponentsCollection _componentsCollection;

        /// <summary>
        /// Initializes new instance of the <see cref="SaveISHComponentsAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="componentsCollection">The collection of InfoShare components</param>
        public SaveISHComponentsAction(ILogger logger, ISHFilePath filePath, ISHComponentsCollection componentsCollection)
            : base(logger, filePath)
        {
            _dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();
            _componentsCollection = componentsCollection;
        }

        /// <summary>
        /// Initializes new instance of the <see cref="SaveISHComponentsAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="componentName">The InfoShare component name</param>
        /// <param name="isEnabled">The status of Component. True if Enabled</param>
        public SaveISHComponentsAction(ILogger logger, ISHFilePath filePath, ISHComponentName componentName, bool isEnabled)
            : base(logger, filePath)
        {
            _dataAggregateHelper = ObjectFactory.GetInstance<IDataAggregateHelper>();

            _componentsCollection = _dataAggregateHelper.ReadComponentsFromFile(FilePath);
            _componentsCollection[componentName].IsEnabled = isEnabled;
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
                _dataAggregateHelper.SaveComponents(FilePath, new ISHComponentsCollection());
            }
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            _dataAggregateHelper.SaveComponents(FilePath, _componentsCollection);
        }
    }
}
