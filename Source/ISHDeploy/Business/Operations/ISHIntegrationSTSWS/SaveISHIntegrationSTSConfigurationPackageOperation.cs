using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.Template;
using ISHDeploy.Data.Managers;
using ISHDeploy.Extensions;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWS
{
    /// <summary>
    /// Saves current STS integration configuration to zip package.
    /// </summary>
    /// <seealso cref="OperationPaths" />
    /// <seealso cref="IOperation" />
    public class SaveISHIntegrationSTSConfigurationPackageOperation : OperationPaths, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveISHIntegrationSTSConfigurationPackageOperation"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="deployment">The instance of <see cref="ISHDeployment"/>.</param>
        /// <param name="fileName">Name of the file.</param>
        public SaveISHIntegrationSTSConfigurationPackageOperation(ILogger logger, Models.ISHDeployment deployment, string fileName)
        {
            _invoker = new ActionInvoker(logger, "Saving STS integration configuration");

            var packageFilePath = Path.Combine(deployment.GetDeploymenPackagesFolderPath(), fileName);
            var temporaryFolder = Path.Combine(Path.GetTempPath(), fileName);
            var temporaryCertificateFilePath = Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.ISHWSCertificateFileName);
            var temporaryDocFilePath = Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName);
            var certificateContent = string.Empty;

            _invoker.AddAction(new DirectoryCreateAction(logger, temporaryFolder));
            _invoker.AddAction(new FileSaveThumbprintAsCertificateAction(logger, temporaryCertificateFilePath, InfoShareSTSConfig.Path.AbsolutePath, InfoShareSTSConfig.CertificateThumbprintXPath));
            _invoker.AddAction(new FileReadAllTextAction(logger, temporaryCertificateFilePath, result => certificateContent = result));
            _invoker.AddAction(new SaveCMSecurityTokenServiceAction(logger, temporaryDocFilePath, deployment.AccessHostName, deployment.GetCMWebAppName(), deployment.GetWSWebAppName(), TemporarySTSConfigurationFileNames.ISHWSCertificateFileName, certificateContent));
            _invoker.AddAction(new DirectoryCreateZipPackageAction(logger, packageFilePath, temporaryFolder));
            _invoker.AddAction(new DirectoryRemoveAction(logger, temporaryFolder));
        }

        /// <summary>
        /// Runs current operation.
        /// </summary>
        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
