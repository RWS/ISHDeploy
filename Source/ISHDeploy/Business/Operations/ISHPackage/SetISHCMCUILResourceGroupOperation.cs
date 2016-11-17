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
using System.Linq;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.ISHUIElement;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
using ISHDeploy.Models.UI.CUIFConfig;

namespace ISHDeploy.Business.Operations.ISHPackage
{
    /// <summary>
    /// Set resource group in ~\Author\ASP\UI\Extensions\_config.xml.
    /// </summary>
    /// <seealso cref="IOperation" />
    public class SetISHCMCUILResourceGroupOperation : BaseOperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="name">The name of resource group.</param>
        /// <param name="relativePaths">Relative paths to resource files.</param>
        public SetISHCMCUILResourceGroupOperation(ILogger logger, Models.ISHDeployment ishDeployment, string name, string[] relativePaths) :
            base(logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, $"Setting resource group in {CUIFConfigFilePath.RelativePath}");

            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            var xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();

            var resourceGroups = xmlConfigManager.Deserialize<ResourceGroups>(CUIFConfigFilePath.AbsolutePath, "resourceGroups");

            var resourceGroup = resourceGroups?.resources == null
                ? new ResourceGroup2 { files = new List<ResourceGroupsResourceGroupFile2>(), name = name }
                : resourceGroups.resources.SingleOrDefault(x => x.name.ToLower() == name.ToLower()) ?? new ResourceGroup2 { files = new List<ResourceGroupsResourceGroupFile2>(), name = name };

            resourceGroup.ChangeItemProperties(CUIFConfigFilePath.RelativePath);

            foreach (var relativePath in relativePaths)
            {
                string absolutePath = Path.Combine(AuthorAspCustomFolderPath, relativePath);

                if (fileManager.FileExists(absolutePath))
                {
                    // Update ~\Author\ASP\UI\Extensions\_config.xml
                    string fileName = $@"../../Custom/{relativePath.Replace(@"\", "/")}";
                    if (resourceGroup.files.All(x => x.name != fileName))
                    {
                        resourceGroup.files.Add(new ResourceGroupsResourceGroupFile2
                        {
                            name = fileName
                        });
                    }

                    _invoker.AddAction(new SetUIElementAction(Logger,
                                    new ISHFilePath(WebFolderPath, BackupWebFolderPath, resourceGroup.RelativeFilePath), resourceGroup));
                }
                else
                {
                    throw new FileNotFoundException($"Could not find file {absolutePath}");
                }
            }
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}