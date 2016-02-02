using System.Collections.Generic;
using InfoShare.Deployment.Cmdlets.ISHUIContentEditor;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces.Commands;
using InfoShare.Deployment.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoShare.Deployment.Tests.Cmdlets.ISHUIContentEditor
{
    [TestClass]
    public class DisableISHUIContentEditorCmdletTest : BaseTest
    {
        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord()
        {
            var cmdlet = new DisableISHUIContentEditorCmdlet
            {
                IshProject = new ISHProject { InstallPath = TestProjectPath }
            };

            string filePathFolderButtonbar = GetPathToFile(ISHPaths.FolderButtonbar);

            var uncommentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut
            };

            ICommand commentCommand = new XmlUncommentCommand(Logger, filePathFolderButtonbar, uncommentPatterns);
            commentCommand.Execute();

            Assert.IsNotNull(GetXElementByXPath(filePathFolderButtonbar, XPathFolderButtonbarCheckOutWithXopusButton), $"{XPathFolderButtonbarCheckOutWithXopusButton} should be uncommented!");
            Assert.IsNotNull(GetXElementByXPath(filePathFolderButtonbar, XPathFolderButtonbarUndoCheckOutButton), $"{XPathFolderButtonbarUndoCheckOutButton} should be uncommented!");

            var result = cmdlet.Invoke();

            foreach (var item in result)
            {

            }

            Assert.IsNull(GetXElementByXPath(GetPathToFile(ISHPaths.FolderButtonbar), XPathFolderButtonbarCheckOutWithXopusButton), $"{XPathFolderButtonbarCheckOutWithXopusButton} should be uncommented!");
            Assert.IsNull(GetXElementByXPath(GetPathToFile(ISHPaths.FolderButtonbar), XPathFolderButtonbarUndoCheckOutButton), $"{XPathFolderButtonbarUndoCheckOutButton} should be uncommented!");
        }
    }
}
