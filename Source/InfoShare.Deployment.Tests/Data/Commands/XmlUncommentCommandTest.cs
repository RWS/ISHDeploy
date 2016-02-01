using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Cmdlets.ISHUIContentEditor;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoShare.Deployment.Tests.Data.Commands
{
    [TestClass]
    public class XmlUncommentCommandTest : BaseCommandTest
    {
        [TestMethod]
        [TestCategory("Commands")]
        public void Execute()
        {
            var filePath = GetPathToFile(ISHPaths.FolderButtonbar);
            var commentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut,
            };

            ICommand uncommentCommand = new XmlUncommentCommand(Logger, filePath, commentPatterns);
            ICommand commentCommand = new XmlCommentCommand(Logger, filePath, commentPatterns);

            XDocument doc = XDocument.Load(filePath);
            var checkOutWithXopusButton = doc.XPathSelectElement(XPathCheckOutWithXopusButton);

            if (checkOutWithXopusButton != null)
            {
                commentCommand.Execute();
                doc = XDocument.Load(filePath);
                checkOutWithXopusButton = doc.XPathSelectElement(XPathCheckOutWithXopusButton);
                Assert.IsNotNull(checkOutWithXopusButton, "Comment command doesn't work");
            }


            uncommentCommand.Execute();

            doc = XDocument.Load(filePath);
            checkOutWithXopusButton = doc.XPathSelectElement(XPathCheckOutWithXopusButton);
            Assert.IsNotNull(checkOutWithXopusButton, "Uncomment command doesn't work");

            var undoCheckOutButton = doc.XPathSelectElement(XPathUndoCheckOutButton);
            Assert.IsNotNull(undoCheckOutButton, "Uncomment command doesn't work");
        }
    }

}
