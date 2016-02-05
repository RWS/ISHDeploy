using System;
using System.IO;
using System.Security.AccessControl;
using InfoShare.Deployment.Cmdlets.ISHContentEditor;
using InfoShare.Deployment.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoShare.Deployment.Tests.Cmdlets.ISHContentEditor
{
    [TestClass]
    public class TestISHContentEditorCmdletTest : BaseTest
    {
        public TestISHContentEditorCmdletTest()
        {

        }

        [TestInitialize]
        public void Initialize()
        {

        }

		[TestMethod]
		[TestCategory("Cmdlets")]
		public void ProcessRecord()
		{
			var cmdlet = new TestISHContentEditorCmdlet
			{
				Hostname = "localhost",
				IshProject = this.IshProject
			};

			var result = cmdlet.Invoke();

			foreach (var item in result)
			{
				bool isValid;
				Assert.IsTrue(Boolean.TryParse(item.ToString(), out isValid), "Result must be a boolean");
			}
		}
	}
}
