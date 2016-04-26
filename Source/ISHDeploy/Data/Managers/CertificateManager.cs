using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
            _logger.WriteDebug($"Getting the certificate with thumbprint: {thumbprint}");
            var certStore = new X509Store(StoreName.My);

            certStore.Open(OpenFlags.ReadOnly);
            
            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            
            if (certCollection.Count == 0)
            {
                throw new ArgumentNullException($"Certificate with thumbprint `{thumbprint}` cannot be found.");
            }
            _logger.WriteDebug($"Found {certCollection.Count} certificates.");

            var builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(certCollection[0].Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }
    }
}
