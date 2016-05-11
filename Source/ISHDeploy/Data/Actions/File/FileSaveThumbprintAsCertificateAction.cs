using ISHDeploy.Data.Managers;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Saves certificate public key to file.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class FileSaveThumbprintAsCertificateAction : SingleFileCreationAction
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
        /// Initializes a new instance of the <see cref="FileSaveThumbprintAsCertificateAction"/> class.
        /// Reads certificate thumbprint from xml file by xpath and uses it to retrieve certificate public key from X509Store.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="certificateFilePath">The certificate file path</param>
        /// <param name="thumbprintFilePath">The certificate thumbprint file path.</param>
        /// <param name="thumbprintXPath">The certificate thumbprint xpath.</param>
        public FileSaveThumbprintAsCertificateAction(ILogger logger, string certificateFilePath, string thumbprintFilePath, string thumbprintXPath):
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
