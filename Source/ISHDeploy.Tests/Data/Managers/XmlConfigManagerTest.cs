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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ISHDeploy.Business.Enums;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Models.ISHXmlNodes;
using ISHDeploy.Models.UI;

namespace ISHDeploy.Tests.Data.Managers
{
    [TestClass]
    public class XmlConfigManagerTest : BaseUnitTest
    {
        private IXmlConfigManager _xmlConfigManager;
        private const string _filePath = "C:\\DummyFilePath.txt";

        [TestInitialize]
        public void TestInitializer()
        {
            _xmlConfigManager = new XmlConfigManager(Logger);
        }

        #region Uncomment Block/Node

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNodesByPrecedingPattern_Enable_XOPUS()
        {
            // Arrange
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern START";
            string endCommentPattern = "testCommentPattern END";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- " + endCommentPattern + " --></BUTTONBAR>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = GetXElementByXPath(document, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']")));

            // Act
            _xmlConfigManager.UncommentNodesByPrecedingPattern(_filePath, testCommentPattern);

            // Assert
            Assert.IsNotNull(result, "Uncommented node should NOT be null");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentNodesByPrecedingPattern_The_structure_of_a_file_does_not_match_with_expected()
        {
            // Arrange
            string testCommentPattern = "testCommentPattern";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR></BUTTONBAR><!-- " + testCommentPattern + " -->");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.UncommentNodesByPrecedingPattern(_filePath, testCommentPattern);

            // Assert
            Assert.Fail("Should throw exception");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNodesByInnerPattern_Enable_Enrich()
        {
            // Arrange
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<!-- " + testCommentPattern +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      testCommentPattern + "--> " +
                                      "</config>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(x, $"*/*[local-name()='javascript'][@src='{testSrc}']")));

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, testCommentPattern);

            // Assert
            Assert.IsNotNull(result, "Uncommented node should NOT be null");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNodesByInnerPattern_Enable_Enrich_contains_already_uncommented_element()
        {
            // Arrange
            string testSrc = "Begin BlueLion integration";
            string testCommentPattern = "<javascript src=\"" + testSrc + "\"";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                     "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                     "</config>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, testCommentPattern);

            // Arrange
            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteVerbose($"{_filePath} contains already uncommented node by searched pattern '{testCommentPattern}'.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentNodesByInnerPattern_Enable_XOPUS_does_not_any_search_pattern()
        {
            // Arrange
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<!--<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON> -->" +
                                    "</BUTTONBAR>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, testCommentPattern);

            // Assert
            Assert.Fail("Should throw exception");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentNodesByInnerPattern_Enable_Enrich_does_not_contains_pattern()
        {
            // Arrange
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                     "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                     "</config>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, testCommentPattern);

            // Assert
            Assert.Fail("Should throw exception");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNodesByInnerPattern_bluelion_config_xml()
        {
            // Arrange
            string testCommentPattern = "../BlueLion-Plugin/create-toolbar.xml";
            string testSrcXPath = "*/*[local-name()='import'][@src='" + testCommentPattern + "']";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                        "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                            "<!--Begin Bluelion integration" +
                                                "<x:import src='" + testCommentPattern + "'/>" +
                                           "End Bluelion integration -->" +
                                         "</x:config>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(x, testSrcXPath)));

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, testCommentPattern);

            // Assert
            Assert.IsNotNull(result, "Uncommented node should NOT be null");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNodesByInnerPattern_Twice_uncomment()
        {
            // Arrange
            string testCommentPattern = "../BlueLion-Plugin/create-toolbar.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                        "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                            "<x:import src='" + testCommentPattern + "'/>" +
                                         "</x:config>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, testCommentPattern);

            // Assert
            Logger.Received(1).WriteVerbose($"{_filePath} contains already uncommented node by searched pattern '{testCommentPattern}'.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentNodesByInnerPattern_Uncomment_if_does_not_contain_pattern()
        {
            // Arrange
            string testCommentPattern = "../BlueLion-Plugin/create-toolbar.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                        "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                         "</x:config>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, testCommentPattern);

            // Assert
            Assert.Fail("Should throw expection");
        }

        #endregion UncommentNode

        #region CommentNode
        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNodesByPrecedingPattern_Disable_XOPUS()
        {
            // Arrange
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern START";
            string endCommentPattern = "testCommentPattern END";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<!-- " + testCommentPattern + " --><BUTTONBAR>" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON><!-- " + endCommentPattern + " -->" +
                                    "</BUTTONBAR>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(x, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']")));

            // Act
            _xmlConfigManager.CommentNodesByPrecedingPattern(_filePath, testCommentPattern);

            // Assert
            Assert.IsNull(result, "Uncommented node is null");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_Disable_Enrich()
        {
            // Arrange
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testXPath = "*/*[local-name()='javascript'][@src='" + testSrc + "']";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='" + testSrc + "' eval=\"false\" phase=\"Xopus\" />" +
                                      "</config>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(x, testXPath)));

            // Act
            _xmlConfigManager.CommentNode(_filePath, testXPath);

            // Assert
            Assert.IsNull(result, "Commented node should be null");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void CommentNodesByPrecedingPattern_Disable_XOPUS_does_not_contains_pattern()
        {
            // Arrange
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "</BUTTONBAR>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.CommentNodesByPrecedingPattern(_filePath, testCommentPattern);

            // Assert
            Assert.Fail("Should throw exception");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_Disable_Enrich_does_not_contain_uncommented_node_within_the_xpath()
        {
            // Arrange
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testXPath = "*/*[local-name()='javascript'][@src='testtest']";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      "</config>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.CommentNode(_filePath, testXPath);

            // Assert
            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteVerbose($"{_filePath} does not contain uncommented node within the xpath {testXPath}");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNodesByPrecedingPattern_Disable_XOPUS_contains_already_commented_part_within_the_pattern()
        {
            // Arrange
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern START";
            string endCommentPattern = "testCommentPattern END";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- " + endCommentPattern + " --></BUTTONBAR>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.CommentNodesByPrecedingPattern(_filePath, testCommentPattern);

            // Assert
            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteVerbose($"{_filePath} contains already commented node following after pattern {testCommentPattern}");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_bluelion_config_xml()
        {
            // Arrange
            string testSrc = "../BlueLion-Plugin/create-toolbar.xml";
            string testXPath = "*/*[local-name()='import'][@src='" + testSrc + "']";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                       "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                               "<x:import src='" + testSrc + "'/>" +
                                        "</x:config>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(x => GetXElementByXPath(doc, testXPath)));

            // Act
            _xmlConfigManager.CommentNode(_filePath, testXPath);

            // Assert
            Assert.IsNull(result, "Commented node should be null");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_Commment_with_encoding()
        {
            // Arrange
            string testButtonName = "TranslationJob";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<BUTTON>" +
                                            @"<!-- Translation\Job -->" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "</BUTTONBAR>");

            XNode result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = document.DescendantNodes().Single(node => node.NodeType == XmlNodeType.Comment)));

            // Act
            _xmlConfigManager.CommentNode(_filePath, $"BUTTONBAR/BUTTON[INPUT[@NAME='{testButtonName}']]", true);

            // Assert
            Assert.IsTrue(result.ToString().Contains(@"<!-\- Translation\\Job -\->"));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_Comment_without_encoding()
        {
            // Arrange
            string testButtonName = "TranslationJob";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<BUTTON>" +
                                            @"<!-- Translation\Job -->" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "</BUTTONBAR>");

            XNode result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = document.DescendantNodes().Single(node => node.NodeType == XmlNodeType.Comment)));

            // Act
            _xmlConfigManager.CommentNode(_filePath, $"BUTTONBAR/BUTTON[INPUT[@NAME='{testButtonName}']]");

            // Assert
            Assert.IsTrue(result.ToString().Contains(@"<!- - Translation\Job - ->"));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNodeByInnerPattern_Uncommment_with_encoding()
        {
            // Arrange
            string testButtonName = "TranslationJob";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<!--<BUTTON>" +
                                            @"<!-\- Translation\\Job -\->" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>-->" +
                                    "</BUTTONBAR>");

            XNode result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = document.DescendantNodes().Single(node => node.NodeType == XmlNodeType.Comment)));

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, $"NAME='{testButtonName}'", true);

            // Assert
            Assert.IsTrue(string.CompareOrdinal(result.ToString(), @"<!-- Translation\Job -->") == 0);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentNodeByInnerPattern_Uncomment_without_encoding()
        {
            // Arrange
            string testButtonName = "TranslationJob";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<!--<BUTTON>" +
                                            @"<!-\- Translation\\Job -\->" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>-->" +
                                    "</BUTTONBAR>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.UncommentNodesByInnerPattern(_filePath, $"NAME='{testButtonName}'");

            // Assert
            Assert.Fail("This test should fail with exception because of wrong xml structure!");
        }

        #endregion Comment Block/Node

        #region Set value

        [TestMethod]
        [TestCategory("Data handling")]
        public void SetAttributeValue()
        {
            string testXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule/identity/@externalId";
            string testValue = "testValue";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<configuration>" +
                                        "<trisoft.infoshare.web.externalpreviewmodule>" +
                                            "<identity externalId='THE_FISHEXTERNALID_TO_USE' />" +
                                        "</trisoft.infoshare.web.externalpreviewmodule>" +
                                    "</configuration>");

            FileManager.Load(_filePath).Returns(doc);
            FileManager.When(x => x.Save(_filePath, doc)).Do(
                        x =>
                        {
                            IEnumerable<object> attributes = (IEnumerable<object>)doc.XPathEvaluate($"{testXPath}");
                            foreach (XAttribute attribute in attributes)
                            {
                                Assert.AreEqual(attribute.Value, testValue, "Setting does NOT work");
                            }
                        }
                    );

            _xmlConfigManager.SetAttributeValue(_filePath, testXPath, testValue);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void SetElementValue()
        {
            string testXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule/identity";
            string testValue = "testValue";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<configuration>" +
                                        "<trisoft.infoshare.web.externalpreviewmodule>" +
                                            "<identity>THE_FISHEXTERNALID_TO_USE</identity>" +
                                        "</trisoft.infoshare.web.externalpreviewmodule>" +
                                    "</configuration>");

            FileManager.Load(_filePath).Returns(doc);
            FileManager.When(x => x.Save(_filePath, doc)).Do(
                        x =>
                        {
                            var element = doc.XPathSelectElement(testXPath);
                            Assert.AreEqual(element.Value, testValue, "Setting does NOT work");
                        }
                    );

            _xmlConfigManager.SetElementValue(_filePath, testXPath, testValue);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void SetElementValue_does_not_contain_element()
        {
            string testXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule/identity2";
            string testValue = "testValue";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<configuration>" +
                                        "<trisoft.infoshare.web.externalpreviewmodule>" +
                                            "<identity>THE_FISHEXTERNALID_TO_USE</identity>" +
                                        "</trisoft.infoshare.web.externalpreviewmodule>" +
                                    "</configuration>");

            FileManager.Load(_filePath).Returns(doc);
            _xmlConfigManager.SetElementValue(_filePath, testXPath, testValue);
            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteWarning(Arg.Is($"{_filePath} does not contain element '{testXPath}'."));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void GetValue_Get_attribute_value_by_xpath()
        {
            // Arrange
            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<node>" +
                            "<childNode nodeAttribute='AttributeValue1'>" +
                                "<someNode>SomeNodeValue</someNode>" +
                            "</childNode>" +
                        "</node>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            var attributeValue = _xmlConfigManager.GetValue(_filePath, "/node/childNode/@nodeAttribute");

            // Assert
            Assert.AreEqual(attributeValue, "AttributeValue1");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void GetValue_Get_node_value_by_xpath()
        {
            // Arrange
            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<node>" +
                            "<childNode nodeAttribute='AttributeValue1'>" +
                                "<someNode>SomeNodeValue</someNode>" +
                            "</childNode>" +
                        "</node>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            var elementValue = _xmlConfigManager.GetValue(_filePath, "/node/childNode/someNode");

            // Assert
            Assert.AreEqual(elementValue, "SomeNodeValue");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXPathException))]
        public void GetValue_Invalid_xpath()
        {
            // Arrange
            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                        "<node>" +
                            "<childNode nodeAttribute='AttributeValue1'>" +
                                "<someNode>SomeNodeValue</someNode>" +
                            "</childNode>" +
                        "</node>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.GetValue(_filePath, "/node/wrongNodeName");

            // Assert
            Assert.Fail("This method should throw exception");
        }

        #endregion

        #region Nodes Manipulation


        private readonly string _nodesManipulationTestXml = $@"<?xml version='1.0' encoding='UTF-8'?>
										<menubar>
										  <!-- Synchronize To LiveContent ============================================================= -->
										  <menuitem label='Synch To Collaborative Review' action='EventMonitor/Main/Overview?' icon='~/UIFramework/synchronization.32.color.png'>
											<userrole>Administrator</userrole>
											<description>Synchronize To SDL Knowledge Center Collaborative Review</description>
										  </menuitem>
										  <!-- Thumbnails ============================================================= -->
										  <menuitem label='Thumbnails' action='EventMonitor/Main/Overview?' icon='~/UIFramework/thumbnails.32x32.png'>
											<userrole>Administrator</userrole>
											<description>Thumbnails</description>
										  </menuitem>
										  <!-- Index ================================================================== -->
										  <menuitem label='All Events' action='EventMonitor/Main/Overview?' icon='~/UIFramework/events.32x32.png'>
											<userrole>Administrator</userrole>
											<description>all processes</description>
										  </menuitem>
										</menubar>";


        [TestMethod]
        [TestCategory("Data handling")]
        public void SetNode_New()
        {
            // Arrange
            string testXPath = "/menubar/menuitem[@label='TEST_Label']";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            var item = new EventLogMenuItem()
            {
                Label = "TEST_Label",
                Description = "TEST_Description",
                Icon = "TEST_icon.png",
                UserRoles = new string[] { "TEST_UserRole1", "TEST_UserRole2" },
                Action = new EventLogMenuItemAction()
                {
                    SelectedButtonTitle = "TEST_SelectedButtonTitle",
                    ModifiedSinceMinutesFilter = 8888,
                    SelectedMenuItemTitle = "TEST_SelectedMenuItemTitle",
                    StatusFilter = "TEST_StatusFilter",
                    EventTypesFilter = new[] { "TEST_REACH", "TEST_PDF", "TEST_ZIP" }
                }
            };

            XComment comment = null;
            Dictionary<string, XAttribute> attributes = new Dictionary<string, XAttribute>();
            IEnumerable<XElement> elements = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    comment = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}{"/preceding-sibling::node()[not(self::text())][1][not(local-name())]"}")).OfType<XComment>().Single();
                    attributes = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/@*")).OfType<XAttribute>().ToDictionary(x => x.Name.LocalName, x => x);
                    elements = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/*")).OfType<XElement>();
                }));

            // Act
            _xmlConfigManager.SetNode(_filePath, testXPath, item);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            Assert.IsNotNull(comment, "Comments was not added");
            Assert.AreEqual(comment.Value, item.GetNodeComment().Value, "Comments was not correctly set");

            Assert.AreEqual(attributes.Count, 3, "Attributes are not set");

            string discription = elements.Where(x => x.Name.LocalName == "description").Select(x => x.Value).First();
            string[] userroles = elements.Where(x => x.Name.LocalName == "userrole").Select(x => x.Value).ToArray();

            Assert.AreEqual(attributes["label"].Value, item.Label, "Label attribute is not set correctly");
            Assert.AreEqual(attributes["icon"].Value, item.Icon, "Label attribute is not set correctly");
            Assert.AreEqual(attributes["action"].Value, item.Action.ToQueryString(), "Action attribute is not set correctly");

            Assert.AreEqual(elements.Count(), 3, "Elements are not set");
            Assert.AreEqual(userroles[0], item.UserRoles[0], "User role element is not set correctly");
            Assert.AreEqual(userroles[1], item.UserRoles[1], "User role element is not set correctly");
            Assert.AreEqual(discription, item.Description, "Description element is not set correctly");
        }


        [TestMethod]
        [TestCategory("Data handling")]
        public void SetNode_Existing()
        {
            // Arrange
            string testXPath = "/menubar/menuitem[@label='Thumbnails']";

            var commentValue = " Thumbnails ============================================================= ";
            var doc = XDocument.Parse(_nodesManipulationTestXml);

            var item = new EventLogMenuItem()
            {
                Label = "Thumbnails",
                Description = "TEST_Description",
                Icon = "TEST_icon.png",
                UserRoles = new string[] { "TEST_UserRole", "TEST_UserRole2" },
                Action = new EventLogMenuItemAction()
                {
                    SelectedButtonTitle = "TEST_SelectedButtonTitle",
                    ModifiedSinceMinutesFilter = 8888,
                    SelectedMenuItemTitle = "TEST_SelectedMenuItemTitle",
                    StatusFilter = "TEST_StatusFilter",
                    EventTypesFilter = new[] { "TEST_REACH", "TEST_PDF", "TEST_ZIP" }
                }
            };

            XComment comment = null;
            Dictionary<string, XAttribute> attributes = new Dictionary<string, XAttribute>();
            IEnumerable<XElement> elements = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    comment = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}{"/preceding-sibling::node()[not(self::text())][1][not(local-name())]"}")).OfType<XComment>().Single();
                    attributes = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/@*")).OfType<XAttribute>().ToDictionary(x => x.Name.LocalName, x => x);
                    elements = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/*")).OfType<XElement>();
                }));

            // Act
            _xmlConfigManager.SetNode(_filePath, testXPath, item);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            Assert.IsNotNull(comment, "Comments was not added");

            Assert.AreNotEqual(comment.Value, item.GetNodeComment().Value, "Comment should not be owerwriten");
            Assert.AreEqual(comment.Value, commentValue, "Comments was not correctly set");

            Assert.AreEqual(attributes.Count, 3, "Attributes are not set");
            Assert.AreEqual(attributes["label"].Value, item.Label, "Label attribute is not set correctly");
            Assert.AreEqual(attributes["icon"].Value, item.Icon, "Label attribute is not set correctly");
            Assert.AreEqual(attributes["action"].Value, item.Action.ToQueryString(), "Action attribute is not set correctly");

            string discription = elements.Where(x => x.Name.LocalName == "description").Select(x => x.Value).First();
            string[] userroles = elements.Where(x => x.Name.LocalName == "userrole").Select(x => x.Value).ToArray();

            Assert.AreEqual(attributes["label"].Value, item.Label, "Label attribute is not set correctly");
            Assert.AreEqual(attributes["icon"].Value, item.Icon, "Label attribute is not set correctly");
            Assert.AreEqual(attributes["action"].Value, item.Action.ToQueryString(), "Action attribute is not set correctly");

            Assert.AreEqual(elements.Count(), 3, "Elements are not set");
            Assert.AreEqual(userroles[0], item.UserRoles[0], "User role element is not set correctly");
            Assert.AreEqual(userroles[1], item.UserRoles[1], "User role element is not set correctly");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveBeforeNode()
        {
            // Arrange
            string testLabel = "All Events";
            string insertBeforeLabel = "Thumbnails";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            string[] labels = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    labels = xdoc.Root.Elements("menuitem").Select(x => x.Attribute("label").Value).ToArray();
                }));

            // Act
            _xmlConfigManager.MoveBeforeNode(
                _filePath,
                string.Format("/menubar/menuitem[@label='{0}']", testLabel),
                string.Format("/menubar/menuitem[@label='{0}']", insertBeforeLabel));

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            Assert.AreEqual(labels.Length, 3, "Nodes quantity was not kept.");
            Assert.AreEqual(labels[1], testLabel, "Inserted node is not in a correct place");
            Assert.AreEqual(labels[2], insertBeforeLabel, "Previouse node is not in a correct place");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveBeforeNode_Top()
        {
            // Arrange
            string testLabel = "All Events";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            string[] labels = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    labels = xdoc.Root.Elements("menuitem").Select(x => x.Attribute("label").Value).ToArray();
                }));

            // Act
            _xmlConfigManager.MoveBeforeNode(
                _filePath,
                string.Format("/menubar/menuitem[@label='{0}']", testLabel));

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            Assert.AreEqual(labels.Length, 3, "Nodes quantity was not kept.");
            Assert.AreEqual(labels[0], testLabel, "Inserted node is not in a correct place");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveAfterNode()
        {
            // Arrange
            string testLabel = "Thumbnails";
            string insertBeforeLabel = "All Events";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            string[] labels = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    labels = xdoc.Root.Elements("menuitem").Select(x => x.Attribute("label").Value).ToArray();
                }));

            // Act
            _xmlConfigManager.MoveAfterNode(
                _filePath,
                string.Format("/menubar/menuitem[@label='{0}']", testLabel),
                string.Format("/menubar/menuitem[@label='{0}']", insertBeforeLabel));

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            Assert.AreEqual(labels.Length, 3, "Nodes quantity was not kept.");
            Assert.AreEqual(labels[2], testLabel, "Inserted node is not in a correct place");
            Assert.AreEqual(labels[1], insertBeforeLabel, "Previouse node is not in a correct place");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveAfterNode_Bottom()
        {
            // Arrange
            string testLabel = "Thumbnails";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            string[] labels = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    labels = xdoc.Root.Elements("menuitem").Select(x => x.Attribute("label").Value).ToArray();
                }));

            // Act
            _xmlConfigManager.MoveAfterNode(
                _filePath,
                string.Format("/menubar/menuitem[@label='{0}']", testLabel));

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            Assert.AreEqual(labels.Length, 3, "Nodes quantity was not kept.");
            Assert.AreEqual(labels[2], testLabel, "Inserted node is not in a correct place");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void RemoveNode()
        {
            // Arrange
            string testLabel = "Thumbnails";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            string[] labels = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    labels = xdoc.Root.Elements("menuitem").Select(x => x.Attribute("label").Value).ToArray();
                }));

            // Act
            _xmlConfigManager.RemoveSingleNode(
                _filePath,
                string.Format("/menubar/menuitem[@label='{0}']", testLabel));

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            Assert.AreEqual(labels.Length, 2, "Node was not removed.");
            Assert.IsFalse(labels.Contains(testLabel), "Wrong node was removed.");
        }


        [TestMethod]
        [TestCategory("Data handling")]
        public void RemoveNode_if_node_has_been_already_removed()
        {
            // Arrange
            string testLabel = "Thumbnails";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            string[] labels = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    labels = xdoc.Root.Elements("menuitem").Select(x => x.Attribute("label").Value).ToArray();
                }));

            // Act
            _xmlConfigManager.RemoveSingleNode(
                _filePath,
                string.Format("/menubar/menuitem[@label='{0}']", testLabel));

            _xmlConfigManager.RemoveSingleNode(
                _filePath,
                string.Format("/menubar/menuitem[@label='{0}']", testLabel));

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(2).WriteVerbose(Arg.Any<string>());

            Assert.AreEqual(labels.Length, 2, "Node was not removed.");
            Assert.IsFalse(labels.Contains(testLabel), "Wrong node was removed.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void RemoveNodes()
        {
            // Arrange
            string testXPath = "/menubar/menuitem/userrole";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            XElement[] elements = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    elements = xdoc.Root.Elements("menuitem").Elements("userrole").ToArray();
                }));

            // Act
            _xmlConfigManager.RemoveNodes(_filePath, testXPath);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Assert.AreEqual(elements.Length, 0, "Nodes was not removed.");
        }


        [TestMethod]
        [TestCategory("Data handling")]
        public void RemoveNodes_if_nodes_are_not_exists()
        {
            // Arrange
            string testXPath = "/tabbar/menuitem/userrole";

            var doc = XDocument.Parse(_nodesManipulationTestXml);

            XElement[] elements = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(
                xdoc =>
                {
                    elements = xdoc.Root.Elements("menuitem").Elements("userrole").ToArray();
                }));

            // Act
            _xmlConfigManager.RemoveNodes(_filePath, testXPath);

            // Assert
            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteVerbose(Arg.Any<string>());

            Assert.IsNull(elements, "Wrong node was removed.");
        }

        #endregion

        #region Insert new node

        [TestMethod]
        [TestCategory("Data handling")]
        public void InsertBeforeNode()
        {
            // Arrange
            string relativeNodeXPath = "configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']";
            string removeNodeXPath = "configuration/system.webServer/staticContent/remove[@fileExtension='.json']";
            string nodeAsXmlString = "<remove fileExtension='.json'/>";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <configuration>
                                            <system.webServer>                                                
                                              <staticContent>
                                                <mimeMap fileExtension='.json' mimeType='text/json' />
                                              </staticContent>
                                            </system.webServer>
                                        </configuration>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = GetXElementByXPath(document, removeNodeXPath)));

            // Act
            _xmlConfigManager.InsertBeforeNode(_filePath, relativeNodeXPath, nodeAsXmlString);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Assert.IsNotNull(result, "Node has not been added");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void InsertBeforeNode_element_has_been_already_added()
        {
            // Arrange
            string relativeNodeXPath = "configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']";
            string nodeAsXmlString = "<remove fileExtension='.json'/>";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <configuration>
                                            <system.webServer>                                                
                                              <staticContent>
                                                <remove fileExtension='.json'/>
                                                <mimeMap fileExtension='.json' mimeType='text/json' />
                                              </staticContent>
                                            </system.webServer>
                                        </configuration>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.InsertBeforeNode(_filePath, relativeNodeXPath, nodeAsXmlString);

            // Assert
            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteVerbose(Arg.Is($"The element with xpath '{relativeNodeXPath}' already contains element '{nodeAsXmlString}' before it."));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(XmlException))]
        public void InsertBeforeNode_XmlException_if_xml_string_of_new_Node_is_empty()
        {
            // Arrange
            string relativeNodeXPath = "configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']";
            string nodeAsXmlString = "";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <configuration>
                                            <system.webServer>                                                
                                              <staticContent>
                                                <mimeMap fileExtension='.json' mimeType='text/json' />
                                              </staticContent>
                                            </system.webServer>
                                        </configuration>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.InsertBeforeNode(_filePath, relativeNodeXPath, nodeAsXmlString);

            // Assert
            Assert.Fail("Exception is expected");
        }


        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXPathException))]
        public void InsertBeforeNode_WrongXPathException()
        {
            // Arrange
            string relativeNodeXPath = "configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']";
            string nodeAsXmlString = "<remove fileExtension='.json'/>";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <configuration>
                                            <system.webServer>                                                
                                              <staticContent>
                                                <mimeMap fileExtension='.json2' mimeType='text/json' />
                                              </staticContent>
                                            </system.webServer>
                                        </configuration>");

            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.InsertBeforeNode(_filePath, relativeNodeXPath, nodeAsXmlString);

            // Assert
            Assert.Fail("Exception is expected");
        }
        #endregion

        #region InsertOrUpdateUIElement

        [TestMethod]
        [TestCategory("Data handling")]
        public void InsertOrUpdateUIElement_simple_adding()
        {
            // Arrange
            var item = new MainMenuBarItem("Test Menu Item", new[] {"Administrator"}, "TestAction.asp");
            string resultNodeXPath = $"mainmenubar/menuitem[@label='{item.Label}']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = GetXElementByXPath(document, resultNodeXPath)));

            // Act
            _xmlConfigManager.InsertOrUpdateUIElement(_filePath, item);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Assert.IsNotNull(result, "Node has not been added");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void InsertOrUpdateUIElement_update()
        {
            // Arrange
            var item = new MainMenuBarItem("Test Menu Item", new[] { "Administrator" }, "TestAction.asp");
            string resultNodeXPath = $"mainmenubar/menuitem[@label='{item.Label}']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = GetXElementByXPath(document, resultNodeXPath)));

            // Act
            _xmlConfigManager.InsertOrUpdateUIElement(_filePath, item);

            item.UserRoles = new[] {"Reader", "Writer"};
            _xmlConfigManager.InsertOrUpdateUIElement(_filePath, item);

            // Assert
            Assert.IsNotNull(result, "Node has not been added");
            Assert.IsTrue(result.XPathSelectElements("userrole").Count() == 2, "Node has not been updated");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void InsertOrUpdateUIElement_insert_first_element()
        {
            // Arrange
            var item = new MainMenuBarItem("Test Menu Item", new[] { "Administrator" }, "TestAction.asp");
            string resultNodeXPath = $"mainmenubar/menuitem[@label='{item.Label}']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar></mainmenubar>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = GetXElementByXPath(document, resultNodeXPath)));

            // Act
            _xmlConfigManager.InsertOrUpdateUIElement(_filePath, item);

            // Assert
            Assert.IsNotNull(result, "Node has not been added");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(XmlException))]
        public void InsertOrUpdateUIElement_insert_wrong_xml()
        {
            // Arrange
            var item = new MainMenuBarItem("Test Menu Item", new[] { "Administrator" }, "TestAction.asp");
            string resultNodeXPath = $"mainmenubar/menuitem[@label='{item.Label}']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        ");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = GetXElementByXPath(document, resultNodeXPath)));

            // Act
            _xmlConfigManager.InsertOrUpdateUIElement(_filePath, item);

            // Assert
            Assert.Fail("Exception is expected");
        }

        #endregion

        #region MoveUIElement

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveUIElement_simple_move_to_the_last_position()
        {
            // Arrange
            string result1NodeXPath = "mainmenubar/menuitem[@label='Event Log 1']";
            string result3NodeXPath = "mainmenubar/menuitem[@label='Event Log 3']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            XElement result1 = null;
            XElement result3 = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document =>
            {
                result1 = GetXElementByXPath(document, result1NodeXPath);
                result3 = GetXElementByXPath(document, result3NodeXPath);
            }));

            // Act
            _xmlConfigManager.MoveUIElement(_filePath, new MainMenuBarItem("Event Log 1"), UIElementMoveDirection.Last);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            XNodeEqualityComparer equalityComparer = new XNodeEqualityComparer();
            
            Assert.IsTrue(equalityComparer.Equals(result1.PreviousNode, result3) , "Node has not been moved");
            Assert.IsNull(result1.NextNode, "Node has not been moved");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveUIElement_simple_move_to_the_first_position()
        {
            // Arrange
            string result1NodeXPath = "mainmenubar/menuitem[@label='Event Log 1']";
            string result3NodeXPath = "mainmenubar/menuitem[@label='Event Log 3']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            XElement result1 = null;
            XElement result3 = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document =>
            {
                result1 = GetXElementByXPath(document, result1NodeXPath);
                result3 = GetXElementByXPath(document, result3NodeXPath);
            }));

            // Act
            _xmlConfigManager.MoveUIElement(_filePath, new MainMenuBarItem("Event Log 3"), UIElementMoveDirection.First);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            XNodeEqualityComparer equalityComparer = new XNodeEqualityComparer();

            Assert.IsTrue(equalityComparer.Equals(result3.NextNode, result1), "Node has not been moved");
            Assert.IsNull(result3.PreviousNode, "Node has not been moved");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveUIElement_simple_move_after()
        {
            // Arrange
            string result1NodeXPath = "mainmenubar/menuitem[@label='Event Log 1']";
            string result2NodeXPath = "mainmenubar/menuitem[@label='Event Log 2']";
            string result3NodeXPath = "mainmenubar/menuitem[@label='Event Log 3']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            XElement result1 = null;
            XElement result2 = null;
            XElement result3 = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document =>
            {
                result1 = GetXElementByXPath(document, result1NodeXPath);
                result2 = GetXElementByXPath(document, result2NodeXPath);
                result3 = GetXElementByXPath(document, result3NodeXPath);
            }));

            // Act
            _xmlConfigManager.MoveUIElement(_filePath, new MainMenuBarItem("Event Log 3"), UIElementMoveDirection.After, new MainMenuBarItem("Event Log 1").XPath);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

            XNodeEqualityComparer equalityComparer = new XNodeEqualityComparer();

            Assert.IsTrue(equalityComparer.Equals(result3.NextNode, result2), "Node has not been moved");
            Assert.IsTrue(equalityComparer.Equals(result3.PreviousNode, result1), "Node has not been moved");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveUIElement_simple_move_after_itself()
        {
            // Arrange
            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            FileManager.Load(_filePath).Returns(doc);
            // Act
            _xmlConfigManager.MoveUIElement(_filePath, new MainMenuBarItem("Event Log 2"), UIElementMoveDirection.After, new MainMenuBarItem("Event Log 2").XPath);

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveUIElement_simple_move_after_not_exist()
        {
            // Arrange
            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            FileManager.Load(_filePath).Returns(doc);
            // Act
            _xmlConfigManager.MoveUIElement(_filePath, new MainMenuBarItem("Event Log 2"), UIElementMoveDirection.After, "Event Log 5");

            // Assert
            FileManager.Received(0).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteWarning(Arg.Is("Not able to find the target node"));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void MoveUIElement_simple_move_not_exist_item()
        {
            // Arrange
            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            FileManager.Load(_filePath).Returns(doc);
            // Act
            _xmlConfigManager.MoveUIElement(_filePath, new MainMenuBarItem("Event Log 5"), UIElementMoveDirection.Last);

            // Assert
            FileManager.Received(0).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteWarning(Arg.Is("Not able to find the target node"));
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(XmlException))]
        public void MoveUIElement_simple_move_wrong_xml()
        {
            // Arrange
            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>");

            FileManager.Load(_filePath).Returns(doc);
            // Act
            _xmlConfigManager.MoveUIElement(_filePath, new MainMenuBarItem("Event Log 5"), UIElementMoveDirection.Last);

            // Assert
            Assert.Fail("Exception is expected");
        }

        #endregion


        #region RemoveUIElement

        [TestMethod]
        [TestCategory("Data handling")]
        public void RemoveUIElement_simple_remove()
        {
            // Arrange
            string result1NodeXPath = "mainmenubar/menuitem[@label='Event Log 1']";

            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            XElement result = null;
            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(document => result = GetXElementByXPath(document, result1NodeXPath)));

            // Act
            _xmlConfigManager.RemoveUIElement(_filePath, new MainMenuBarItem("Event Log 1"));

            // Assert
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Assert.IsNull(result, "Node has not been removed");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void RemoveUIElement_not_exist()
        {
            // Arrange
            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>
                                        <mainmenubar>
                                            <menuitem label='Event Log 1' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 2' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                            <menuitem label='Event Log 3' action='testAction.asp' id='EVENTLOG'>
                                                <userrole>Administrator</userrole>
                                            </menuitem>
                                        </mainmenubar>");

            FileManager.Load(_filePath).Returns(doc);
            // Act
            _xmlConfigManager.RemoveUIElement(_filePath, new MainMenuBarItem("Event Log 5"));

            // Assert
            FileManager.Received(0).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteWarning(Arg.Is("Not able to find the target node"));
        }
        
        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(XmlException))]
        public void RemoveUIElement_simple_move_wrong_xml()
        {
            // Arrange
            var doc = XDocument.Parse(@"<?xml version='1.0' encoding='UTF-8'?>");

            FileManager.Load(_filePath).Returns(doc);
            // Act
            _xmlConfigManager.RemoveUIElement(_filePath, new MainMenuBarItem("Event Log 5"));

            // Assert
            Assert.Fail("Exception is expected");
        }

        #endregion

        #region RemoveUIElement

        [TestMethod]
        [TestCategory("Data handling")]
        public void Serialize()
        {
            // Arrange

            var serializedItem = "<?xml version=\"1.0\" encoding=\"utf-16\"?><menuitem label=\"Event Log 1\" />";
            // Act
            var result = _xmlConfigManager.Serialize(new MainMenuBarItem("Event Log 1"));

            // Assert
            Assert.IsTrue(serializedItem == result, "Node has not been removed");
        }

        #endregion
    }
}
