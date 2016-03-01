using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers;
using InfoShare.Deployment.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using InfoShare.Deployment.Data.Exceptions;
using InfoShare.Deployment.Tests.Extensions;

namespace InfoShare.Deployment.Tests.Data.Managers
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
        public void UncommentBlock_Enable_XOPUS()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "DisabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- " + testCommentPattern + " END --></BUTTONBAR>");
            
            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']");
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );

            _xmlConfigManager.UncommentBlock(testFilePath, testCommentPattern);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentBlock_The_structure_of_a_file_does_not_match_with_expected()
        {
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "DisabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START -->" +
                                    "<!-- " + testCommentPattern + " END --></BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);

            _xmlConfigManager.UncommentBlock(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_Enable_Enrich()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = "DisabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<!-- " + testCommentPattern +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      testCommentPattern + "--> " +
                                      "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"*/*[local-name()='javascript'][@src='{testSrc}']");
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );
            
            _xmlConfigManager.UncommentNode(testFilePath, testCommentPattern);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_Enable_XOPUS_does_not_contain_commented_part_within_the_pattern()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "DisabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START -->" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "<!-- XOPUS ADD 'CHECK OUT WITH XOPUS' END --></BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        Assert.Fail("Saving should not happen");
                    }
                );
            
            _xmlConfigManager.UncommentBlock(testFilePath, testCommentPattern);

            Logger.Received(1).WriteWarning($"{testFilePath} does not contain start and end pattern '{testCommentPattern}' where it's expected.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_Enable_Enrich_contains_already_uncommented_element()
        {
            string testSrc = "Begin BlueLion integration";
            string testCommentPattern = "<javascript src=\"" + testSrc + "\"";
            var testFilePath = "DisabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                     "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                     "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x => Assert.Fail("Saving should not happen")
                );

            _xmlConfigManager.UncommentNode(testFilePath, testCommentPattern);
            
            Logger.Received(1).WriteVerbose($"{testFilePath} contains already uncommented element '{testCommentPattern}'.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_Enable_XOPUS_does_not_contains_start_or_end_pattern()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            string testFilePath = "DisabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<!--<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON> -->" +
                                    "</BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        Assert.Fail("Saving should not happen");
                    }
                );
            
            _xmlConfigManager.UncommentBlock(testFilePath, testCommentPattern);

            Logger.Received(1).WriteWarning($"{testFilePath} does not contain start and end pattern '{testCommentPattern}' where it's expected.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentNode_Enable_Enrich_does_not_contains_pattern()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = "DisabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                     "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                     "</config>");

            FileManager.Load(testFilePath).Returns(doc);

            _xmlConfigManager.UncommentNode(testFilePath, testCommentPattern);

            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteWarning($"{testFilePath} does not contain pattern '{testCommentPattern}' where it's expected.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_bluelion_config_xml()
        {
            string testCommentPattern = "../BlueLion-Plugin/create-toolbar.xml";
            string testSrcXPath = "*/*[local-name()='import'][@src='" + testCommentPattern + "']";
            var testFilePath = "bluelion-config.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                        "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                            "<!--Begin Bluelion integration" +
                                                "<x:import src='" + testCommentPattern + "'/>" +
                                           "End Bluelion integration -->" +
                                         "</x:config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, testSrcXPath);
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );
            
            _xmlConfigManager.UncommentNode(testFilePath, testCommentPattern);

            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.DidNotReceive().WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_Twice_uncomment()
        {
            string testCommentPattern = "../BlueLion-Plugin/create-toolbar.xml";
            string testSrcXPath = "*/*[local-name()='import'][@src='" + testCommentPattern + "']";
            var testFilePath = "bluelion-config.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                        "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                            "<x:import src='" + testCommentPattern + "'/>" +
                                         "</x:config>");

            FileManager.Load(testFilePath).Returns(doc);

            _xmlConfigManager.UncommentNode(testFilePath, testCommentPattern);

            Logger.Received(1).WriteVerbose($"{testFilePath} contains already uncommented element '{testCommentPattern}'.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        [ExpectedException(typeof(WrongXmlStructureException))]
        public void UncommentNode_Uncomment_if_does_not_contain_pattern()
        {
            string testCommentPattern = "../BlueLion-Plugin/create-toolbar.xml";
            string testSrcXPath = "*/*[local-name()='import'][@src='" + testCommentPattern + "']";
            var testFilePath = "bluelion-config.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                        "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                         "</x:config>");

            FileManager.Load(testFilePath).Returns(doc);

            _xmlConfigManager.UncommentNode(testFilePath, testCommentPattern);
        }

        #endregion UncommentNode
        
        #region CommentNode
        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_Disable_XOPUS()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "EnabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<!-- " + testCommentPattern + " START --><BUTTONBAR>" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON><!-- " + testCommentPattern + " END -->" +
                                    "</BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']");
                        Assert.IsNull(element, "Uncommented node is null");
                    }
                );
            
            _xmlConfigManager.CommentBlock(testFilePath, testCommentPattern);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }
        
        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_Disable_Enrich()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testXPath = "*/*[local-name()='javascript'][@src='" + testSrc + "']";
            var testFilePath = "EnabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='" + testSrc + "' eval=\"false\" phase=\"Xopus\" />" +
                                      "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, testXPath);
                        Assert.IsNull(element, "Commented node should be null");
                    }
                );
            
            _xmlConfigManager.CommentNode(testFilePath, testXPath);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_Disable_XOPUS_does_not_contains_pattern()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "EnabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR>" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "</BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x => Assert.Fail("Saving should not happen")
                );
            
            _xmlConfigManager.CommentBlock(testFilePath, testCommentPattern);

            Logger.Received(1).WriteWarning($"{testFilePath} does not contain start and end pattern '{testCommentPattern}' where it's expected.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_Disable_Enrich_does_not_contain_uncommented_node_within_the_xpath()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testXPath = "*/*[local-name()='javascript'][@src='testtest']";
            var testFilePath = "EnabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      "</config>");

            FileManager.Load(testFilePath).Returns(doc);

            _xmlConfigManager.CommentNode(testFilePath, testXPath);
            
            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteVerbose($"{testFilePath} does not contain uncommented node within the xpath {testXPath}");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_Disable_XOPUS_contains_already_commented_part_within_the_pattern()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "EnabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- " + testCommentPattern + " END --></BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);

            _xmlConfigManager.CommentBlock(testFilePath, testCommentPattern);

            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteVerbose($"{testFilePath} contains already commented part within the pattern {testCommentPattern}");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_Disable_XOPUS_does_not_contain_ending_pattern()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "EnabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START -->" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "</BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);
            
            _xmlConfigManager.CommentBlock(testFilePath, testCommentPattern);

            FileManager.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<XDocument>());
            Logger.Received(1).WriteWarning($"{testFilePath} does not contain start and end pattern '{testCommentPattern}' where it's expected.");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_bluelion_config_xml()
        {
            string testSrc = "../BlueLion-Plugin/create-toolbar.xml";
            string testXPath = "*/*[local-name()='import'][@src='" + testSrc + "']";

            var testFilePath = "bluelion-config.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\"?>" +
                                       "<x:config xmlns:x=\"http://www.xopus.com/xmlns/config\" version=\"1.0\">" +
                                               "<x:import src='" + testSrc + "'/>" +
                                        "</x:config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, testXPath);
                        Assert.IsNull(element, "Commented node should be null");
                    }
                );

            _xmlConfigManager.CommentNode(testFilePath, testXPath);

            FileManager.Received(1).Save(testFilePath, doc);
            Logger.DidNotReceive().WriteVerbose($"{testFilePath} does not contain uncommented node within the xpath {testXPath}");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_Testing_specific_TranslationExternalCommentXPath_xpath()
        {
            // Arrange
            var doc = XDocument.Parse(
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BUTTONBAR>
    <!-- {CommentPatterns.TranslationComment} -->
    <BUTTON>
        <INPUT type=""button""/>
    </BUTTON>
</BUTTONBAR>");

            XDocument result = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(docResult => result = docResult));

            _xmlConfigManager.CommentNode(_filePath, CommentPatterns.TranslationExternalCommentXPath);

            // Assert
            var expected = XDocument.Parse(
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BUTTONBAR>
    <!-- {CommentPatterns.TranslationComment} -->
    <!--<BUTTON>
        <INPUT type=""button"" />
    </BUTTON>-->
</BUTTONBAR>");

            Assert.IsTrue(result.IsSameAs(expected), "Expected document is different from actual result");
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNodeWithInternalPattern_Internal_paths_exist()
        {
            // Arrange
            var doc = XDocument.Parse(
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BUTTONBAR>
    <BUTTON>
        <!-- {CommentPatterns.TranslationComment} -->
        <INPUT type=""button""/>
    </BUTTON>
</BUTTONBAR>");
            XDocument result = null;

            FileManager.Load(_filePath).Returns(doc);
            FileManager.Save(_filePath, Arg.Do<XDocument>(docResult => result = docResult));

            // Act
            _xmlConfigManager.CommentNodeWithInternalPattern(_filePath, CommentPatterns.TranslationInternalCommentXPath, CommentPatterns.TranslationComment);

            // Assert
            var expected = XDocument.Parse(
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BUTTONBAR>
    <!-- {CommentPatterns.TranslationComment} -->
    <!--<BUTTON>
        <INPUT type=""button"" />
    </BUTTON>-->
</BUTTONBAR>");
            
            Assert.IsTrue(result.IsSameAs(expected), "Expected document is different from actual result");
        }
        
        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNodeWithInternalPattern_Path_does_not_exist()
        {
            // Arrange
            var doc = XDocument.Parse(
$@"<?xml version=""1.0"" encoding=""UTF-8""?>
<BUTTONBAR>
    <!-- {CommentPatterns.TranslationComment} -->
    <BUTTON>
        <INPUT type=""button""/>
    </BUTTON>
</BUTTONBAR>");
            
            FileManager.Load(_filePath).Returns(doc);

            // Act
            _xmlConfigManager.CommentNodeWithInternalPattern(_filePath, CommentPatterns.TranslationInternalCommentXPath, CommentPatterns.TranslationComment);

            // Assert
            Logger.Received(1).WriteVerbose($"{_filePath} does not contain searched xpath {CommentPatterns.TranslationInternalCommentXPath}");
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
            var testFilePath = "Tesweb.config";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<configuration>" +
                                        "<trisoft.infoshare.web.externalpreviewmodule>" +
                                            "<identity externalId='THE_FISHEXTERNALID_TO_USE' />" +
                                        "</trisoft.infoshare.web.externalpreviewmodule>" +
                                    "</configuration>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                        x =>
                        {
                            IEnumerable<object> attributes = (IEnumerable<object>)doc.XPathEvaluate($"{testXPath}/@{testAttributeName}");
                            foreach (XAttribute attribute in attributes)
                            {
                                Assert.AreEqual(attribute.Value, testValue, "Setting does NOT work");
                            }
                        }
                    );

            _xmlConfigManager.SetAttributeValue(testFilePath, testXPath, testAttributeName, testValue);
            FileManager.Received(1).Save(Arg.Any<string>(), Arg.Any<XDocument>());
        }

        #endregion
    }
}
