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
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Certificate
{
    /// <summary>
    /// Saves certificate public key to file.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class SaveThumbprintAsCertificateAction : SingleFileCreationAction
    {
        /// <summary>
        /// The certificate file path.
        /// </summary>
        private readonly string _certificateFilePath;

        /// <summary>
        /// The thumbprint file path.
        /// </summary>
        private readonly string _thumbprintFilePath;

        /// <summary>
        /// The thumbprint xpath.
        /// </summary>
        private readonly string _thumbprintXPath;

        /// <summary>
        /// The xml configuration manager.
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;
        
        /// <summary>
        /// The certificate manager.
        /// </summary>
        private readonly ICertificateManager _certificateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveThumbprintAsCertificateAction"/> class.
        /// Reads certificate thumbprint from xml file by xpath and uses it to retrieve certificate public key from X509Store.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="certificateFilePath">The certificate file path</param>
        /// <param name="thumbprintFilePath">The certificate thumbprint file path.</param>
        /// <param name="thumbprintXPath">The certificate thumbprint xpath.</param>
        public SaveThumbprintAsCertificateAction(ILogger logger, string certificateFilePath, string thumbprintFilePath, string thumbprintXPath):
            base(logger, certificateFilePath)
        {
            _certificateFilePath = certificateFilePath;
            _thumbprintFilePath = thumbprintFilePath;
            _thumbprintXPath = thumbprintXPath;

            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
            _certificateManager = ObjectFactory.GetInstance<ICertificateManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var thumbprint = _xmlConfigManager.GetValue(_thumbprintFilePath, _thumbprintXPath);
            var cerFileContent = _certificateManager.GetCertificatePublicKey(thumbprint);

            FileManager.Write(_certificateFilePath, cerFileContent);
        }
    }
}
