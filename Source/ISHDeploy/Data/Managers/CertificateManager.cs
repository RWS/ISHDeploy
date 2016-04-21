using System;
using System.Security.Cryptography.X509Certificates;

namespace ISHDeploy.Data.Managers
{
    public class CertificateManager
    {
        //TODO: Get PEM format certificate
        public string GetCertificatePublicKey(string thumbprint)
        {
            var certStore = new X509Store(StoreName.My);

            certStore.Open(OpenFlags.ReadOnly);

            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            //builder.AppendLine("-----BEGIN CERTIFICATE-----");
            //builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            //builder.AppendLine("-----END CERTIFICATE-----");

            //return builder.ToString();

            if (certCollection.Count == 0)
            {
                throw new ArgumentNullException($"Certificate with thumbprint `{thumbprint}` cannot be found.");
            }

            return certCollection[0].GetPublicKeyString();
        }
    }
}
