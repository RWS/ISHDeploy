
namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Operates on registered certificates.
    /// </summary>
    public interface ICertificateManager
    {
        /// <summary>
        /// Gets the certificate public key.
        /// </summary>
        /// <param name="thumbprint">The certificate thumbprint.</param>
        /// <returns>Certificate public key.</returns>
        string GetCertificatePublicKey(string thumbprint);
    }
}
