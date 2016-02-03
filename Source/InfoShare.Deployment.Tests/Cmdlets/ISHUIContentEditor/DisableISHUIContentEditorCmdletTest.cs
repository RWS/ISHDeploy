using System.IO;
using InfoShare.Deployment.Cmdlets.ISHUIContentEditor;
using InfoShare.Deployment.Data;
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
                IshProject = this.IshProject
            };

            foreach (var file in Directory.GetFiles(".\\TestData\\ISHUIContentEditor\\XSL\\Enabled"))
            {
                File.Copy(file, Path.Combine(@"TestData\Web\Author\ASP\XSL", Path.GetFileName(file)), true);
            }

            var result = cmdlet.Invoke();

            foreach (var item in result) { }

            Assert.IsNull(GetXElementByXPath(GetPathToFile(ISHPaths.FolderButtonbar), XPathCheckOutWithXopusButton), $"{XPathCheckOutWithXopusButton} in file {ISHPaths.FolderButtonbar} should be commented!");
            Assert.IsNull(GetXElementByXPath(GetPathToFile(ISHPaths.FolderButtonbar), XPathUndoCheckOutButton), $"{XPathUndoCheckOutButton} in file {ISHPaths.FolderButtonbar} should be commented!");
            Assert.IsNull(GetXElementByXPath(GetPathToFile(ISHPaths.InboxButtonBar), XPathCheckOutWithXopusButton), $"{XPathCheckOutWithXopusButton} in file {ISHPaths.InboxButtonBar} should be commented!");
            Assert.IsNull(GetXElementByXPath(GetPathToFile(ISHPaths.LanguageDocumentButtonBar), XPathCheckOutWithXopusButton), $"{XPathCheckOutWithXopusButton} in file {ISHPaths.LanguageDocumentButtonBar} should be commented!");
            Assert.IsNotNull(GetXElementByXPath(GetPathToFile(ISHPaths.InboxButtonBar), XPathCheckOutButton), $"{XPathCheckOutButton} in file {ISHPaths.InboxButtonBar} should be uncommented!");
            Assert.IsNotNull(GetXElementByXPath(GetPathToFile(ISHPaths.InboxButtonBar), XPathCheckOutButton), $"{XPathCheckOutButton} in file {ISHPaths.InboxButtonBar} should be uncommented!");
            Assert.IsNotNull(GetXElementByXPath(GetPathToFile(ISHPaths.LanguageDocumentButtonBar), XPathCheckOutButton), $"{XPathCheckOutButton} in file {ISHPaths.LanguageDocumentButtonBar} should be uncommented!");
            Assert.IsNotNull(GetXElementByXPath(GetPathToFile(ISHPaths.LanguageDocumentButtonBar), XPathCheckOutButton), $"{XPathCheckOutButton} in file {ISHPaths.LanguageDocumentButtonBar} should be uncommented!");
        }
    }
}
