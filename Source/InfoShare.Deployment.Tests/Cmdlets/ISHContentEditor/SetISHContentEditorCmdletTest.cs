using System;
using System.IO;
using System.Security.AccessControl;
using InfoShare.Deployment.Cmdlets.ISHContentEditor;
using InfoShare.Deployment.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoShare.Deployment.Tests.Cmdlets.ISHContentEditor
{
    [TestClass]
    public class SetISHContentEditorCmdletTest : BaseTest
    {
        private readonly string _newLicenceFilePath;

        public SetISHContentEditorCmdletTest()
        {
            _newLicenceFilePath = GetPathToFile($"{ISHPaths.LicenceFolderPath}\\127.0.0.1.txt");
        }

        [TestInitialize]
        public void Initialize()
        {
            if (File.Exists(_newLicenceFilePath))
            {
                File.SetAttributes(_newLicenceFilePath, File.GetAttributes(_newLicenceFilePath) & ~FileAttributes.ReadOnly); // write/read right
                File.Delete(_newLicenceFilePath);
            }

            Assert.IsFalse(File.Exists(_newLicenceFilePath), "File has not been deleted");
        }


        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord()
        {
            var cmdlet = new SetISHContentEditorCmdlet
            {
                LicensePath = $"{this.IshProject.WebPath}\\127.0.0.1.txt",
                IshProject = this.IshProject
            };

            var result = cmdlet.Invoke();

            foreach (var item in result) { }

            Assert.IsTrue(File.Exists(_newLicenceFilePath), "File has not been copied");
        }

        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord_ForceCopy()
        {
            var cmdlet = new SetISHContentEditorCmdlet
            {
                LicensePath = $"{this.IshProject.WebPath}\\127.0.0.1.txt",
                Force = true,
                IshProject = this.IshProject
            };
            
            File.Copy(cmdlet.LicensePath, _newLicenceFilePath);
            var writeTime = DateTime.Now;
            File.SetLastWriteTime(_newLicenceFilePath, writeTime);
            
            var result = cmdlet.Invoke();
            foreach (var item in result) { }

            var newWriteTime = new FileInfo(_newLicenceFilePath).LastWriteTime;

            Assert.IsTrue(File.Exists(_newLicenceFilePath), "File has not been copied");
            Assert.IsTrue(writeTime.CompareTo(newWriteTime) != 0, "File has not been overwritten");
        }

        [TestMethod]
        [TestCategory("Cmdlets")]
        [ExpectedException(typeof(IOException))]
        public void ProcessRecord_AlreadyExists()
        {
            var cmdlet = new SetISHContentEditorCmdlet
            {
                LicensePath = $"{this.IshProject.WebPath}\\127.0.0.1.txt",
                IshProject = this.IshProject
            };

            File.Copy(cmdlet.LicensePath, _newLicenceFilePath);
            var writeTime = DateTime.Now;
            File.SetLastWriteTime(_newLicenceFilePath, writeTime);

            var result = cmdlet.Invoke();
            foreach (var item in result) { }
        }
        
        [TestMethod]
        [TestCategory("Cmdlets")]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void ProcessRecord_ReadOnly()
        {
            var cmdlet = new SetISHContentEditorCmdlet
            {
                LicensePath = $"{this.IshProject.WebPath}\\127.0.0.1.txt",
                Force = true,
                IshProject = this.IshProject
            };
            
            File.Copy(cmdlet.LicensePath, _newLicenceFilePath);
            File.SetAttributes(_newLicenceFilePath, FileAttributes.ReadOnly); // readonly

            var result = cmdlet.Invoke();
            foreach (var item in result) { }
        }
        
        [TestMethod]
        [TestCategory("Cmdlets")]
        [ExpectedException(typeof(IOException))]
        public void ProcessRecord_LockFile()
        {
            var cmdlet = new SetISHContentEditorCmdlet
            {
                LicensePath = $"{this.IshProject.WebPath}\\127.0.0.1.txt",
                Force = true,
                IshProject = this.IshProject
            };

            File.Copy(cmdlet.LicensePath, _newLicenceFilePath);

            using (var fs = File.Open(_newLicenceFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var result = cmdlet.Invoke();
                foreach (var item in result) { }
            }
        }
    }
}
