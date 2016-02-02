using System.Collections.Generic;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoShare.Deployment.Tests.Data.Commands
{
    [TestClass]
    public class XmlCommentCommandTest : BaseTest
    {
        [TestMethod]
        [TestCategory("Commands")]
        [DeploymentItem("TestData", "TestData")]
        public void Execute()
        {
            var filePath = GetPathToFile(ISHPaths.FolderButtonbar);
            var commentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut
            };

            ICommand uncommentCommand = new XmlUncommentCommand(Logger, filePath, commentPatterns);
            ICommand commentCommand = new XmlCommentCommand(Logger, filePath, commentPatterns);

            var checkOutWithXopusButton = GetXElementByXPath(filePath, XPathFolderButtonbarCheckOutWithXopusButton);

            if (checkOutWithXopusButton == null)
            {
                uncommentCommand.Execute();
                checkOutWithXopusButton = GetXElementByXPath(filePath, XPathFolderButtonbarCheckOutWithXopusButton);
                Assert.IsNotNull(checkOutWithXopusButton, "Uncomment command doesn't work");
            }

            commentCommand.Execute();

            checkOutWithXopusButton = GetXElementByXPath(filePath, XPathFolderButtonbarCheckOutWithXopusButton);
            Assert.IsNull(checkOutWithXopusButton, "Comment command doesn't work");

            var undoCheckOutButton = GetXElementByXPath(filePath, XPathFolderButtonbarUndoCheckOutButton);
            Assert.IsNull(undoCheckOutButton, "Comment command doesn't work");
        }
    }

}
