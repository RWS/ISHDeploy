using System;
using ISHDeploy.Data.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISHDeploy.Tests.Data.Managers
{
    [TestClass]
    public class CertificateManagerTest
    {
        [TestMethod]
        public void GetCertificatePublicKey_Works()
        {
            var certManager = new CertificateManager();

            var str = certManager.GetCertificatePublicKey("13018856a5af9528c13b47f2521c0a36f85cdc95");
        }
    }
}
