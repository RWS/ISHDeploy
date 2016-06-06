using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.WebAdministration
{
    [TestClass]
    public class RecycleApplicationPoolActionTest : BaseUnitTest
    {
        private readonly IWebAdministrationManager _webAdministrationManager = Substitute.For<IWebAdministrationManager>();

        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance(_webAdministrationManager);
        }

       
        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_RecycleApplicationPool()
        {
            // Arrange
            var appPoolName = "TestAppPool";

            // Act
            (new RecycleApplicationPoolAction(Logger, appPoolName)).Execute();

            // Assert
            _webAdministrationManager.Received(1).RecycleApplicationPool(appPoolName);
            Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
        }
    }
}
