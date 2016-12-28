/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using ISHDeploy.Common;
using ISHDeploy.Data.Actions.Certificate;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions.File
{
    [TestClass]
    public class SaveCertificateActionTest : BaseUnitTest
    {
        private ICertificateManager _certificateManager;
        private IXmlConfigManager _xmlConfigManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _certificateManager = Substitute.For<ICertificateManager>();
            _xmlConfigManager = Substitute.For<IXmlConfigManager>();

            ObjectFactory.SetInstance(_certificateManager);
            ObjectFactory.SetInstance(_xmlConfigManager);
        }

        [TestMethod]
        [TestCategory("Actions")]
        public void Execute_Certificate_information_retrieved_from_xml_file()
        {
            // Arrange
            var certFilePath = "C:\\Packages\\cert.cer";
            var certContent = "someDummyCertificateContent";
            var thumbprintFile = "C:\\Instance\\config.xml";
            var thumbprintXPath = "someXPath";
            var thumbprint = "someDummyThumbprint";

            var action = new SaveThumbprintAsCertificateAction(Logger, certFilePath, thumbprintFile, thumbprintXPath);
            _xmlConfigManager.GetValue(thumbprintFile, thumbprintXPath).Returns(thumbprint);
            _certificateManager.GetCertificatePublicKey(thumbprint).Returns(certContent);

            // Act
            action.Execute();

            // Assert
            FileManager.Received().Write(certFilePath, certContent);
        }
    }
}
