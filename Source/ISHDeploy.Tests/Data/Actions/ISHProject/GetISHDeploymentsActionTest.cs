using System;
using System.Collections.Generic;
using System.Linq;
using ISHDeploy.Data.Actions.ISHProject;
using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Data.Exceptions;
using ISHDeploy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.ISHProject
{
    [TestClass]
    public class GetISHDeploymentsActionTest : BaseUnitTest
    {
        private readonly IRegistryManager _registryManager = Substitute.For<IRegistryManager>();
        private readonly IXmlConfigManager _xmlManager = Substitute.For<IXmlConfigManager>();

        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance(_registryManager);
            ObjectFactory.SetInstance(_xmlManager);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void ExecuteWithResult_Has_0_installed_projects()
        {
            IEnumerable<ISHDeployment> ishProjects = null;
            var cmd = new GetISHDeploymentsAction(Logger, null, res => ishProjects = res);

            _registryManager.GetInstalledProjectsKeys().Returns(new RegistryKey[0]);

            cmd.Execute();

            _registryManager.Received(1).GetInstalledProjectsKeys();
            Logger.Received(1).WriteVerbose(Arg.Any<string>());
            Assert.IsNotNull(ishProjects, "Return value from the action should not be null, it should be empty collection");
            Assert.IsFalse(ishProjects.Any(), "Returned projects number should be 0");
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void ExecuteWithResult_Has_1_installed_project()
        {
            IEnumerable<ISHDeployment> ishProjects = null;
            var cmd = new GetISHDeploymentsAction(Logger, null, res => ishProjects = res);

            _registryManager.GetInstalledProjectsKeys().Returns(new RegistryKey[1]);
            _registryManager.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(@"E:\SomeFakeFolder");
            _registryManager.GetInstalledProjectVersion(Arg.Any<RegistryKey>()).Returns(Version.Parse("1.1.1.0"));
            FileManager.FileExists(Arg.Any<string>()).Returns(true);

            cmd.Execute();

            _registryManager.Received(1).GetInstalledProjectsKeys();
            Logger.DidNotReceive().WriteVerbose(Arg.Any<string>());
            Assert.IsNotNull(ishProjects, "Return value from the action should not be null");
            Assert.AreEqual(ishProjects.Count(), 1, "Returned projects number should be 1");
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void ExecuteWithResult_1_of_3_files_does_not_exist()
        {
            IEnumerable<ISHDeployment> ishProjects = null;
            var cmd = new GetISHDeploymentsAction(Logger, null, res => ishProjects = res);

            var i = 0;
            _registryManager.GetInstalledProjectsKeys().Returns(new RegistryKey[3]);
            _registryManager.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(@"E:\SomeFakeFolder");
            FileManager.FileExists(Arg.Any<string>()).Returns(_ => i++ != 2);

            cmd.Execute();

            _registryManager.Received(3).GetInstalledProjectVersion(Arg.Any<RegistryKey>());
            _registryManager.Received(3).GetInstallParamFilePath(Arg.Any<RegistryKey>());
            Logger.Received(1).WriteError(Arg.Any<CorruptedInstallationException>(), Arg.Any<Object>());
            Assert.IsNotNull(ishProjects, "Return value from the action should not be null");
            Assert.AreEqual(ishProjects.Count(), 2, "Returned projects number should be 2");
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void ExecuteWithResult_Project_with_such_suffix_not_found()
        {
            IEnumerable<ISHDeployment> ishProjects = null;
            string suffix = "suffix1";
            var cmd = new GetISHDeploymentsAction(Logger, suffix, res => ishProjects = res);

            _registryManager.GetInstalledProjectsKeys(suffix).Returns(new RegistryKey[0]);
            FileManager.FileExists(Arg.Any<string>()).Returns(true);

            cmd.Execute();

            _registryManager.Received(1).GetInstalledProjectsKeys(suffix);
            Logger.Received(1).WriteError(Arg.Any<DeploymentNotFoundException>(), suffix);
            Assert.IsNotNull(ishProjects, "Return value from the action should not be null");
            Assert.IsFalse(ishProjects.Any(), "Returned projects number should be 0");
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void ExecuteWithResult_Install_parameteres_file_path_is_null()
        {
            IEnumerable<ISHDeployment> ishProjects = null;
            var cmd = new GetISHDeploymentsAction(Logger, null, res => ishProjects = res);

            _registryManager.GetInstalledProjectsKeys().Returns(new RegistryKey[1]);
            _registryManager.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(_ => null);
            ObjectFactory.SetInstance(new FileManager(Logger));

            cmd.Execute();
            
            _registryManager.Received(1).GetInstallParamFilePath(Arg.Any<RegistryKey>());
            Logger.Received(1).WriteError(Arg.Any<CorruptedInstallationException>(), Arg.Any<Object>());
            Assert.IsNotNull(ishProjects, "Return value from the action should not be null");
            Assert.IsFalse(ishProjects.Any(), "Returned projects number should be 0");
        }
        
        [TestMethod]
        [TestCategory("Actions")]
        public void ExecuteWithResult_Version_is_null()
        {
            IEnumerable<ISHDeployment> ishProjects = null;
            var cmd = new GetISHDeploymentsAction(Logger, null, res => ishProjects = res);

            _registryManager.GetInstalledProjectsKeys().Returns(new RegistryKey[1]);
            _registryManager.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(@"E:\SomeFakeFolder");
            _registryManager.GetInstalledProjectVersion(Arg.Any<RegistryKey>()).Returns(_ => null);
            FileManager.FileExists(Arg.Any<string>()).Returns(true);

            cmd.Execute();
            
            Logger.DidNotReceive().WriteError(Arg.Any<Exception>());
            Assert.IsNotNull(ishProjects, "Return value from the action should not be null");
            Assert.AreEqual(ishProjects.Count(), 1, "Returned projects number should be 1");
        }
    }
}
