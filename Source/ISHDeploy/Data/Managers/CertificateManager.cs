using System;
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
        /// Initializes a new instance of the <see cref="CertificateManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CertificateManager(ILogger logger)
        {
            _logger = logger;
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
        /// Finds the certificate by thumbprint.
        /// </summary>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private X509Certificate2 FindCertificateByThumbprint(string thumbprint)
        {
            _logger.WriteDebug($"Getting the certificate with thumbprint: {thumbprint}");
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
