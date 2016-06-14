
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

        /// <summary>
        /// Gets the encrypted raw data by thumbprint.
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <returns></returns>
        string GetEncryptedRawDataByThumbprint(string thumbprint);
    }
}
