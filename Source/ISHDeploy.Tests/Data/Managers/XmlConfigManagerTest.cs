using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ISHDeploy.Business;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Models;

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
            string testXPath = "configuration/trisoft.infoshare.web.externalpreviewmodule/identity";
            string testAttributeName = "externalId";
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
                            IEnumerable<object> attributes = (IEnumerable<object>)doc.XPathEvaluate($"{testXPath}/@{testAttributeName}");
                            foreach (XAttribute attribute in attributes)
                            {
                                Assert.AreEqual(attribute.Value, testValue, "Setting does NOT work");
                            }
                        }
                    );

            _xmlConfigManager.SetAttributeValue(_filePath, testXPath, testAttributeName, testValue);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
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
				UserRole = "TEST_UserRole",
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
			Dictionary<string, XElement> elements = new Dictionary<string, XElement>();

			FileManager.Load(_filePath).Returns(doc);
			FileManager.Save(_filePath, Arg.Do<XDocument>(
				xdoc =>
				{
					comment = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}{CommentPatterns.EventMonitorPreccedingCommentXPath}")).OfType<XComment>().Single();
					attributes = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/@*")).OfType<XAttribute>().ToDictionary(x => x.Name.LocalName, x => x);
					elements = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/*")).OfType<XElement>().ToDictionary(x => x.Name.LocalName, x => x);
				}));

			// Act
			_xmlConfigManager.SetNode(_filePath, testXPath, item);

			// Assert
			FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

			Assert.IsNotNull(comment, "Comments was not added");
			Assert.AreEqual(comment.Value, item.GetNodeComment().Value, "Comments was not correctly set");

			Assert.AreEqual(attributes.Count, 3, "Attributes are not set");
			Assert.AreEqual(attributes["label"].Value, item.Label, "Label attribute is not set correctly");
			Assert.AreEqual(attributes["icon"].Value, item.Icon, "Label attribute is not set correctly");
			Assert.AreEqual(attributes["action"].Value, item.Action.ToQueryString(), "Action attribute is not set correctly");

			Assert.AreEqual(elements.Count, 2, "Elements are not set");
			Assert.AreEqual(elements["userrole"].Value, item.UserRole, "User role element is not set correctly");
			Assert.AreEqual(elements["description"].Value, item.Description, "Description element is not set correctly");
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
				UserRole = "TEST_UserRole",
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
			Dictionary<string, XElement> elements = new Dictionary<string, XElement>();

			FileManager.Load(_filePath).Returns(doc);
			FileManager.Save(_filePath, Arg.Do<XDocument>(
				xdoc =>
				{
					comment = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}{CommentPatterns.EventMonitorPreccedingCommentXPath}")).OfType<XComment>().Single();
					attributes = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/@*")).OfType<XAttribute>().ToDictionary(x => x.Name.LocalName, x => x);
					elements = ((IEnumerable<object>)xdoc.XPathEvaluate($"{testXPath}/*")).OfType<XElement>().ToDictionary(x => x.Name.LocalName, x => x);
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

			Assert.AreEqual(elements.Count, 2, "Elements are not set");
			Assert.AreEqual(elements["userrole"].Value, item.UserRole, "User role element is not set correctly");
			Assert.AreEqual(elements["description"].Value, item.Description, "Description element is not set correctly");
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
				string.Format(CommentPatterns.EventMonitorTab, testLabel), 
				string.Format(CommentPatterns.EventMonitorTab, insertBeforeLabel));

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
				string.Format(CommentPatterns.EventMonitorTab, testLabel));

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
				string.Format(CommentPatterns.EventMonitorTab, testLabel),
				string.Format(CommentPatterns.EventMonitorTab, insertBeforeLabel));

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
				string.Format(CommentPatterns.EventMonitorTab, testLabel));

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
				string.Format(CommentPatterns.EventMonitorTab, testLabel));

			// Assert
			FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());

			Assert.AreEqual(labels.Length, 2, "Node was not removed.");
			Assert.IsFalse(labels.Contains(testLabel), "Wrong node was removed.");
		}

		#endregion
	}
}
