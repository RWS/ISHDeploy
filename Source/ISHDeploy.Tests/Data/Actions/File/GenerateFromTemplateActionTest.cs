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
﻿using System.Collections.Generic;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class GenerateFromTemplateActionTest : BaseUnitTest
    {
        private ITemplateManager _templateManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _templateManager = Substitute.For<ITemplateManager>();
            ObjectFactory.SetInstance(_templateManager);
        }
        
        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Document_with_correct_parameteres_is_created()
        {
            // Arrange
            var templateFilePath = "input.template";
            var outputFilePath = "out.txt";
            var contentValue = "Generated Document Content";
            var inputParam = new Dictionary<string, string>() {};

            _templateManager.GenerateDocument(templateFilePath, inputParam).Returns(contentValue);

            var action = new FileGenerateFromTemplateAction(Logger, templateFilePath, outputFilePath, inputParam);
            
            // Act
            action.Execute();

            // Assert
            FileManager.Received().Write(outputFilePath, contentValue);
        }
    }
}
