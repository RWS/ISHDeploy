using System.Collections.Generic;
using InfoShare.Deployment.Cmdlets.ISHUIContentEditor;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoShare.Deployment.Tests.Cmdlets.ISHUIContentEditor
{
    [TestClass]
    public class EnableISHUIContentEditorCmdletTest : BaseTest
    {
        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord()
        {
            var cmdlet = new EnableISHUIContentEditorCmdlet
            {
                IshProject = this.IshProject
            };

            string filePathFolderButtonbar = GetPathToFile(ISHPaths.FolderButtonbar);

            var commentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut
            };

            ICommand commentCommand = new XmlCommentCommand(Logger, filePathFolderButtonbar, commentPatterns);
            commentCommand.Execute();

            Assert.IsNull(GetXElementByXPath(filePathFolderButtonbar, XPathFolderButtonbarCheckOutWithXopusButton), $"{XPathFolderButtonbarCheckOutWithXopusButton} should be commented!");
            Assert.IsNull(GetXElementByXPath(filePathFolderButtonbar, XPathFolderButtonbarUndoCheckOutButton), $"{XPathFolderButtonbarUndoCheckOutButton} should be commented!");

            var result = cmdlet.Invoke();

            foreach (var item in result)
            {

            }

            Assert.IsNotNull(GetXElementByXPath(GetPathToFile(ISHPaths.FolderButtonbar), XPathFolderButtonbarCheckOutWithXopusButton), $"{XPathFolderButtonbarCheckOutWithXopusButton} should be uncommented!");
            Assert.IsNotNull(GetXElementByXPath(GetPathToFile(ISHPaths.FolderButtonbar), XPathFolderButtonbarUndoCheckOutButton), $"{XPathFolderButtonbarUndoCheckOutButton} should be uncommented!");
        }
    }
}
