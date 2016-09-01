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
﻿using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Operates on registered certificates.
    /// </summary>
    /// <seealso cref="ICertificateManager" />
    public class CertificateManager : ICertificateManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The file manager.
        /// </summary>
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CertificateManager(ILogger logger)
        {
            _logger = logger;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Gets the certificate public key.
        /// </summary>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <returns>Certificate public key.</returns>
        public string GetCertificatePublicKey(string thumbprint)
        {
            var certificate = FindCertificateByThumbprint(thumbprint);

            var builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(certificate.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        /// <summary>
        /// Gets the certificate subject.
        /// </summary>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <returns>Certificate subject.</returns>
        public string GetCertificateSubjectByThumbprint(string thumbprint)
        {
            var certificate = FindCertificateByThumbprint(thumbprint);

            return certificate.Subject;
        }

        /// <summary>
        /// Gets the encrypted raw data by thumbprint.
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <returns></returns>
        public string GetEncryptedRawDataByThumbprint(string thumbprint)
        {
            var certificate = FindCertificateByThumbprint(thumbprint);

            return Convert.ToBase64String(certificate.RawData);
        }

        /// <summary>
        /// Gets the path to certificate by thumbprint
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <returns></returns>
        public string GetPathToCertificateByThumbprint(string thumbprint)
        {
            _logger.WriteDebug($"Get path to the certificate with thumbprint: {thumbprint}");
            var certificate = FindCertificateByThumbprint(thumbprint);

            var uniqueKeyContainerName =
                ((System.Security.Cryptography.RSACryptoServiceProvider) certificate.PrivateKey).CspKeyContainerInfo
                    .UniqueKeyContainerName;

            var commonApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string path = $"{commonApplicationDataPath}\\Microsoft\\Crypto\\RSA\\MachineKeys\\{uniqueKeyContainerName}";

            if (_fileManager.FileExists(path))
            {
                _logger.WriteDebug($"The path to the certificate: {path}");
                return path;
            }

            var applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = $"{applicationDataPath}\\Microsoft\\Crypto\\RSA\\MachineKeys\\{uniqueKeyContainerName}";
            path = _fileManager.GetFiles(path, uniqueKeyContainerName, true).FirstOrDefault();

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception($"Could not locate private key file for certificate {thumbprint}");
            }

            _logger.WriteDebug($"The path to the certificate: {path}");

            return path;
        }

        /// <summary>
        /// Finds the certificate by thumbprint.
        /// </summary>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private X509Certificate2 FindCertificateByThumbprint(string thumbprint)
        {
            _logger.WriteDebug($"Get the certificate with thumbprint: {thumbprint}");
            var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            certStore.Open(OpenFlags.ReadOnly);

            var certificate = certStore.Certificates.OfType<X509Certificate2>()
                .FirstOrDefault(x => x.Thumbprint == thumbprint.ToUpper());
            certStore.Close();

            if (certificate == null)
            {
                throw new ArgumentNullException($"Certificate with thumbprint `{thumbprint}` cannot be found.");
            }
            _logger.WriteDebug("Certificate has been found.");

            return certificate;
        }
    }
}
