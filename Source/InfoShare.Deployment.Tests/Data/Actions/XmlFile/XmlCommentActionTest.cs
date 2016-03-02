using System.Xml.Linq;
using InfoShare.Deployment.Data.Actions.XmlFile;
using InfoShare.Deployment.Data.Managers;
using InfoShare.Deployment.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Actions.XmlFile
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
            new XmlNodesByPrecedingPatternCommentAction(Logger, testFilePath, testCommentPattern).Execute();

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
            new XmlNodeCommentAction(Logger, testFilePath, testXPath).Execute();

            // Assert
            Assert.IsNull(result, "Comment action doesn't work");
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }
    }
}
