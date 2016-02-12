using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Business.CmdSets.ISHExternalPreview;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Services
{
    [TestClass]
    public class XmlConfigManagerTest : BaseUnitTest
    {
        #region UncommentNode
        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentBlock_EnableXOPUS()
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

            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlConfigManager(Logger).UncommentBlock(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_EnableEnrich()
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
                        var element = GetXElementByXPath(doc, $"*/javascript[@src='{testSrc}']");
                        Assert.IsNotNull(element, "Uncommented node should NOT be null");
                    }
                );


            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlConfigManager(Logger).UncommentNode(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_EnableXOPUS_dose_not_contain_commented_part_within_the_pattern()
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

            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).UncommentBlock(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_EnableEnrich_dose_not_contain_pattern()
        {
            string testSrc = "Begin BlueLion integration";
            string testCommentPattern = "<javascript src='" + testSrc + "'";
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

            Logger.When(x => x.WriteVerbose($"{testFilePath} does not contain pattern '{testCommentPattern}' where it's expected.")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).UncommentNode(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_EnableXOPUS_does_not_contains_start_or_end_pattern()
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

            Logger.When(x => x.WriteVerbose($"{testFilePath}does not contains start or end pattern {testCommentPattern}.")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).UncommentBlock(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_EnableEnrich_does_not_contains_pattern()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = "DisabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                     "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                     "<!-- " + testCommentPattern + "--> " +
                                     "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x => Assert.Fail("Saving should not happen")
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath}does not contains start or end pattern {testCommentPattern}.")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).UncommentNode(testFilePath, testCommentPattern);
        }

        #endregion UncommentNode


        #region CommentNode
        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_DisableXOPUS()
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

            Logger.When(x => x.WriteWarning($"{testFilePath} does not contain start and end pattern '{testCommentPattern}' where it's expected.")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlConfigManager(Logger).CommentBlock(testFilePath, testCommentPattern);
        }


        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_DisableEnrich()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "<javascript src=\"" + testSrc + "\"";
            string testXPath= "config/javascript[@src='" + testSrc + "']";
            var testFilePath = "EnabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src=\"" + testSrc + "\" eval=\"false\" phase=\"Xopus\" />" +
                                      "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"*/javascript[@src='{testSrc}']");
                        Assert.IsNull(element, "Commented node should be null");
                    }
                );


            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlConfigManager(Logger).CommentNode(testFilePath, testXPath);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_DisableXOPUS_does_not_contains_pattern()
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

            Logger.When(x => x.WriteVerbose($"{testFilePath} does not contain start and end pattern '{testCommentPattern}' where it's expected.")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).CommentBlock(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_DisableEnrich_does_not_contain_uncommented_node_within_the_pattern()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "<javascript src=\"123";
            string testXPath = "config/javascript[@src='" + testSrc + "']";
            var testFilePath = "EnabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x => Assert.Fail("Saving should not happen")
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath} does not contain uncommented node within the pattern {testCommentPattern}.")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).CommentNode(testFilePath, testXPath);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_DisableXOPUS_contains_already_commented_part_within_the_pattern()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "EnabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- XOPUS ADD 'CHECK OUT WITH XOPUS' END --></BUTTONBAR>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x => Assert.Fail("Saving should not happen")
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath} contains already commented part within the pattern {testCommentPattern}")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).CommentBlock(testFilePath, testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentBlock_DisableXOPUS_does_not_contain_ending_pattern()
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
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x => Assert.Fail("Saving should not happen")
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath} does not contain ending pattern '{testCommentPattern}' where it's expected.")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger).CommentBlock(testFilePath, testCommentPattern);
        }
        #endregion CommentNode

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

            new XmlConfigManager(Logger).SetAttributeValue(testFilePath, testXPath, testAttributeName, testValue);
        }

        #endregion
    }
}
