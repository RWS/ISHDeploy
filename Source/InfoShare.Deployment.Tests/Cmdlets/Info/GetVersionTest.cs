using System;
using InfoShare.Deployment.Cmdlets.Info;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoShare.Deployment.Tests.Cmdlets.Info
{
    [TestClass]
    public class GetVersionTest
    {
        [TestMethod]
        [TestCategory("Cmdlets")]
        public void ProcessRecord()
        {
            var cmdlet = new GetVersionCmdlet();
            var result = cmdlet.Invoke();

            foreach (var item in result)
            {
                Version version;
                Assert.IsTrue(Version.TryParse(item.ToString(), out version), "The return value must be version");
            }
        }
    }
}
