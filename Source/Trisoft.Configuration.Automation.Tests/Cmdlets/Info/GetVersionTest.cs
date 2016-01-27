using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfoCmdlets = Trisoft.Configuration.Automation.Cmdlets.Info;

namespace Trisoft.Configuration.Automation.Tests.Cmdlets.Info
{
    [TestClass]
    public class GetVersionTest
    {
        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord()
        {
            var cmdlet = new InfoCmdlets.GetVersion();
            var result = cmdlet.Invoke();

            foreach (var item in result)
            {
                Version version;
                Assert.IsTrue(Version.TryParse(item.ToString(), out version), "The return value must be version");
            }
        }
    }
}
