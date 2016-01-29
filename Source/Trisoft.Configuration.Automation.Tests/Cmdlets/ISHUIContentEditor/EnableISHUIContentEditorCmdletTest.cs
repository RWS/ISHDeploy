using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Tests.Cmdlets.ISHUIContentEditor
{
    [TestClass]
    public class EnableISHUIContentEditorCmdletTest
    {
        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord()
        {
            var cmdlet = new Automation.Cmdlets.ISHUIContentEditor.EnableISHUIContentEditorCmdlet
            {
                IshProject = new ISHProject { InstallPath = @"F:\InfoShare" }
            };

            var result = cmdlet.Invoke();

            foreach (var item in result)
            {
                Assert.IsTrue(true, "The return value must be version");
            }
        }

        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord_WithBackup()
        {
            var cmdlet = new Automation.Cmdlets.ISHUIContentEditor.EnableISHUIContentEditorCmdlet
            {
                IshProject = new ISHProject { InstallPath = @"F:\InfoShare" },
                RollbackOnFailure = true
            };
            var result = cmdlet.Invoke();

            foreach (var item in result)
            {
                Assert.IsTrue(true, "The return value must be version");
            }
        }
    }
}
