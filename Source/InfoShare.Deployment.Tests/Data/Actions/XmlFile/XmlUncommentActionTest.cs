using System.Xml.Linq;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Data.Managers;
using InfoShare.Deployment.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.XmlFile
{
    [TestClass]
    public class XmlUncommentActionTest : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance<IXmlConfigManager>(new XmlConfigManager(Logger));
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Enable_XOPUS()
        {
            // Arrange
            string testButtonName = "testDoButton";
            string testCommentPattern = "testCommentPattern START";
            string endCommentPattern = "testCommentPattern END";
            var testFilePath = GetIshFilePath("DisabledXOPUS.xml");

            var doc = XDocument.Parse("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                    "<BUTTONBAR><!-- " + testCommentPattern + " --><!-- Xopus is disabled.Please obtain a license from SDL Trisoft" +
                                        "<BUTTON>" +
                                            "<INPUT type='button' NAME='" + testButtonName + "' />" +
                                        "</BUTTON>" +
                                    "Xopus is disabled.Please obtain a license from SDL Trisoft --><!-- " + endCommentPattern + " --></BUTTONBAR>");

            XElement result = null;
            FileManager.Load(testFilePath.AbsolutePath).Returns(doc);
            FileManager.Save(testFilePath.AbsolutePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(x, $"BUTTONBAR/BUTTON/INPUT[@NAME='{testButtonName}']")));

            // Act
            new XmlNodesByPrecedingPatternUncommentAction(Logger, testFilePath, testCommentPattern).Execute();

            // Assert
            Assert.IsNotNull(result, "Uncommented node should NOT be null");
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Enable_Enrich()
        {
            // Arrange
            string testSrc = "../BlueLion-Plugin/Bootstrap/bootstrap.js";
            string testCommentPattern = "Begin BlueLion integration";
            var testFilePath = this.GetIshFilePath("DisabledEnrich.xml");

            var doc = XDocument.Parse("<config version='1.0' xmlns='http://www.xopus.com/xmlns/config'>" +
                                      "<javascript src='config.js' eval='false' phase='Xopus' />" +
                                      "<javascript src='enhancements.js' eval='false' phase='Xopus' />" +
                                      "<!-- " + testCommentPattern +
                                      "<javascript src='" + testSrc + "' eval='false' phase='Xopus' />" +
                                      testCommentPattern + "--> " +
                                      "</config>");

            XElement result = null;
            FileManager.Load(testFilePath.AbsolutePath).Returns(doc);
            FileManager.Save(testFilePath.AbsolutePath, Arg.Do<XDocument>(x => result = GetXElementByXPath(x, $"*/*[local-name()='javascript'][@src='{testSrc}']")));

            // Act
            new XmlNodesByInnerPatternUncommentAction(Logger, testFilePath, testCommentPattern).Execute();

            // Assert
            Assert.IsNotNull(result, "Uncommented node should NOT be null");
        }
    }
}
