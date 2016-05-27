using Microsoft.VisualStudio.TestTools.UnitTesting;
using ISHDeploy.Cmdlets.ISHDeployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISHDeploy.Cmdlets.ISHDeployment.Tests
{
    [TestClass()]
    public class GetISHDeploymentCmdletTests
    {
        [TestMethod()]
        public void ExecuteCmdletTest()
        {
            GetISHDeploymentCmdlet getISHDeploymentCmdlet = new GetISHDeploymentCmdlet();
            getISHDeploymentCmdlet.ExecuteCmdlet();
            Assert.Fail();
        }
    }
}