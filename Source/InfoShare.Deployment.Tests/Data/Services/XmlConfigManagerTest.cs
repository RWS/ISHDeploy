using System.Xml.Linq;
using InfoShare.Deployment.Data.Services;
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
        public void UncommentNode_EnableXOPUS()
        {
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern";
            var testFilePath = "DisabledXOPUS.xml";

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " START --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- XOPUS ADD 'CHECK OUT WITH XOPUS' END --></BUTTONBAR>");


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

            new XmlConfigManager(Logger, testFilePath).UncommentNode(testCommentPattern);
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

            new XmlConfigManager(Logger, testFilePath).UncommentNode(testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_EnableXOPUS_if_dose_not_contain_commented_part_within_the_pattern()
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

            new XmlConfigManager(Logger, testFilePath).UncommentNode(testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void UncommentNode_EnableEnrich_if_dose_not_contain_commented_part_within_the_pattern()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = "DisabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                     "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                     "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                     "<!-- " + testCommentPattern + "-->" +
                                     "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                     "<!-- " + testCommentPattern + "--> " +
                                     "</config>");

            FileManager.Load(testFilePath).Returns(doc);
            FileManager.When(x => x.Save(testFilePath, doc)).Do(
                    x => Assert.Fail("Saving should not happen")
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger, testFilePath).UncommentNode(testCommentPattern);
        }

        #endregion UncommentNode


        #region CommentNode
        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_DisableXOPUS()
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
                    x =>
                    {
                        var element = GetXElementByXPath(doc, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']");
                        Assert.IsNull(element, "Uncommented node is null");
                    }
                );

            Logger.When(x => x.WriteVerbose($"{testFilePath} dose not contain commented part within the pattern {testCommentPattern}")).Do(
                x => Assert.Fail("Commented node has not been uncommented"));

            new XmlConfigManager(Logger, testFilePath).CommentNode(testCommentPattern);
        }


        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_DisableEnrich()
        {
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = "EnabledEnrich.xml";

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<!-- " + testCommentPattern + "-->" +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      "<!-- " + testCommentPattern + "--> " +
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

            new XmlConfigManager(Logger, testFilePath).CommentNode(testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_DisableXOPUS_does_not_contains_pattern()
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

            Logger.When(x => x.WriteVerbose($"{testFilePath} does not contains pattern {testCommentPattern}.")).Do(
                Assert.IsNotNull);

            new XmlConfigManager(Logger, testFilePath).CommentNode(testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_DisableXOPUS_contains_already_commented_part_within_the_pattern()
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

            new XmlConfigManager(Logger, testFilePath).CommentNode(testCommentPattern);
        }

        [TestMethod]
        [TestCategory("Data handling")]
        public void CommentNode_DisableXOPUS_does_not_contain_ending_pattern()
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

            new XmlConfigManager(Logger, testFilePath).CommentNode(testCommentPattern);
        }
        #endregion CommentNode
    }
}
