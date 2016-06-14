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
﻿using System.Xml.Linq;
using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.XmlFile
{
    [TestClass]
    public class XmlCommentActionTest : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Disable_XOPUS()
        {
            // Arrange
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern START";
            string endCommentPattern = "testCommentPattern END";
            var testFilePath = GetIshFilePath("DisabledXOPUS.xml");

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<!-- " + testCommentPattern + " --><BUTTONBAR>" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON><!-- " + endCommentPattern + " -->" +
                                    "</BUTTONBAR>");
            
            XElement result = null;
            FileManager.Load(testFilePath.AbsolutePath).Returns(doc);
            FileManager.Save(testFilePath.AbsolutePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(doc, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']")));
            
            // Act
            new CommentNodesByPrecedingPatternAction(Logger, testFilePath, testCommentPattern).Execute();

            // Assert
            Assert.IsNull(result, "Uncommented node is null");
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Disable_Enrich()
        {
            // Arrange
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testXPath = "*/*[local-name()='javascript'][@src='" + testSrc + "']";
            var testFilePath = GetIshFilePath("EnabledEnrich.xml");

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='" + testSrc + "' eval=\"false\" phase=\"Xopus\" />" +
                                      "</config>");

            XElement result = null;
            FileManager.Load(testFilePath.AbsolutePath).Returns(doc);
            FileManager.Save(testFilePath.AbsolutePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(doc, testXPath)));

            // Act
            new CommentNodeByXPathAction(Logger, testFilePath, testXPath).Execute();

            // Assert
            Assert.IsNull(result, "Comment action doesn't work");
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }
    }
}
