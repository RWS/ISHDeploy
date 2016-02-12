using System;
using System.Collections.Generic;
using System.Linq;
using InfoShare.Deployment.Data.Commands.ISHProjectCommands;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Exceptions;
using InfoShare.Deployment.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Commands.ISHProjectCommands
{
    [TestClass]
    public class GetISHProjectsCommandTest : BaseUnitTest
    {
        private readonly IRegistryService _registryService = Substitute.For<IRegistryService>();
        private readonly IXmlConfigManager _xmlManager = Substitute.For<IXmlConfigManager>();

        [TestInitialize]
        public void TestInitializer()
        {
            ObjectFactory.SetInstance(_registryService);
            ObjectFactory.SetInstance(_xmlManager);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void ExecuteWithResult_0InstalledProjects()
        {
            IEnumerable<ISHProject> ishProjects = null;
            var cmd = new GetISHProjectsCommand(Logger, null, res => ishProjects = res);

            _registryService.GetInstalledProjectsKeys().Returns(new RegistryKey[0]);

            cmd.Execute();

            _registryService.Received(1).GetInstalledProjectsKeys();
            Logger.Received(1).WriteVerbose(Arg.Any<string>());
            Assert.IsNotNull(ishProjects, "Return value from the command should not be null, it should be empty collection");
            Assert.IsFalse(ishProjects.Any(), "Returned projects number should be 0");
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void ExecuteWithResult_1InstalledProject()
        {
            IEnumerable<ISHProject> ishProjects = null;
            var cmd = new GetISHProjectsCommand(Logger, null, res => ishProjects = res);

            _registryService.GetInstalledProjectsKeys().Returns(new RegistryKey[1]);
            _registryService.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(@"E:\SomeFakeFolder");
            _registryService.GetInstalledProjectVersion(Arg.Any<RegistryKey>()).Returns(Version.Parse("1.1.1.0"));
            FileManager.Exists(Arg.Any<string>()).Returns(true);

            cmd.Execute();

            _registryService.Received(1).GetInstalledProjectsKeys();
            Logger.Received(0).WriteVerbose(Arg.Any<string>());
            Assert.IsNotNull(ishProjects, "Return value from the command should not be null");
            Assert.AreEqual(ishProjects.Count(), 1, "Returned projects number should be 1");
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void ExecuteWithResult_1From3FilesDoesNotExist()
        {
            IEnumerable<ISHProject> ishProjects = null;
            var cmd = new GetISHProjectsCommand(Logger, null, res => ishProjects = res);

            var i = 0;
            _registryService.GetInstalledProjectsKeys().Returns(new RegistryKey[3]);
            _registryService.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(@"E:\SomeFakeFolder");
            FileManager.Exists(Arg.Any<string>()).Returns(_ => i++ != 2);

            cmd.Execute();

            _registryService.Received(3).GetInstalledProjectVersion(Arg.Any<RegistryKey>());
            _registryService.Received(3).GetInstallParamFilePath(Arg.Any<RegistryKey>());
            Logger.Received(1).WriteError(Arg.Any<CorruptedInstallationException>(), Arg.Any<Object>());
            Assert.IsNotNull(ishProjects, "Return value from the command should not be null");
            Assert.AreEqual(ishProjects.Count(), 2, "Returned projects number should be 2");
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void ExecuteWithResult_InstallNotFoundWithSuffix()
        {
            IEnumerable<ISHProject> ishProjects = null;
            string suffix = "suffix1";
            var cmd = new GetISHProjectsCommand(Logger, suffix, res => ishProjects = res);

            _registryService.GetInstalledProjectsKeys(suffix).Returns(new RegistryKey[0]);
            FileManager.Exists(Arg.Any<string>()).Returns(true);

            cmd.Execute();

            _registryService.Received(1).GetInstalledProjectsKeys(suffix);
            Logger.Received(1).WriteError(Arg.Any<DeploymentNotFoundException>(), suffix);
            Assert.IsNotNull(ishProjects, "Return value from the command should not be null");
            Assert.IsFalse(ishProjects.Any(), "Returned projects number should be 0");
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void ExecuteWithResult_InstallParamsPathIsNull()
        {
            IEnumerable<ISHProject> ishProjects = null;
            var cmd = new GetISHProjectsCommand(Logger, null, res => ishProjects = res);

            _registryService.GetInstalledProjectsKeys().Returns(new RegistryKey[1]);
            _registryService.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(_ => null);
            ObjectFactory.SetInstance(new FileManager(Logger));

            cmd.Execute();
            
            _registryService.Received(1).GetInstallParamFilePath(Arg.Any<RegistryKey>());
            Logger.Received(1).WriteError(Arg.Any<CorruptedInstallationException>(), Arg.Any<Object>());
            Assert.IsNotNull(ishProjects, "Return value from the command should not be null");
            Assert.IsFalse(ishProjects.Any(), "Returned projects number should be 0");
        }
        
        [TestMethod]
        [TestCategory("Commands")]
        public void ExecuteWithResult_VersionIsNull()
        {
            IEnumerable<ISHProject> ishProjects = null;
            var cmd = new GetISHProjectsCommand(Logger, null, res => ishProjects = res);

            _registryService.GetInstalledProjectsKeys().Returns(new RegistryKey[1]);
            _registryService.GetInstallParamFilePath(Arg.Any<RegistryKey>()).Returns(@"E:\SomeFakeFolder");
            _registryService.GetInstalledProjectVersion(Arg.Any<RegistryKey>()).Returns(_ => null);
            FileManager.Exists(Arg.Any<string>()).Returns(true);

            cmd.Execute();
            
            Logger.Received(0).WriteError(Arg.Any<Exception>());
            Assert.IsNotNull(ishProjects, "Return value from the command should not be null");
            Assert.AreEqual(ishProjects.Count(), 1, "Returned projects number should be 1");
        }
    }
}
