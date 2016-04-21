using System;
using ISHDeploy.Data.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISHDeploy.Tests.Data.Managers
{
    [TestClass]
    public class PackageManagerTest : BaseUnitTest
    {
        [TestMethod]
        public void PackFolder_Works()
        {
            ObjectFactory.SetInstance(new FileManager(Logger));

            var packageManager = new PackageManager();

            packageManager.PackFolder("C:\\ProgramData\\ISHDeploy.12.0.0\\InfoShare\\Backup");
        }
    }
}
