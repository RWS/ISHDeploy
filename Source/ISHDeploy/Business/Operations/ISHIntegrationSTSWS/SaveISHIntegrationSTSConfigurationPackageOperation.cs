using System.Collections.Generic;
using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
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
        /// Initializes a new instance of the <see cref="SaveISHIntegrationSTSConfigurationPackageOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="deployment">The instance of <see cref="ISHDeployment" />.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="packAdfsInvokeScript">if set to <c>true</c> the add ADFS script invocation into package.</param>
        public SaveISHIntegrationSTSConfigurationPackageOperation(ILogger logger, Models.ISHDeployment deployment, string fileName, bool packAdfsInvokeScript = false)
        {
            _invoker = new ActionInvoker(logger, "Saving STS integration configuration");

            var packageFilePath = Path.Combine(deployment.GetDeploymenPackagesFolderPath(), fileName);
            var temporaryFolder = Path.Combine(Path.GetTempPath(), fileName);
            var temporaryCertificateFilePath = Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.ISHWSCertificateFileName);

            var certificateContent = string.Empty;

            _invoker.AddAction(new DirectoryCreateAction(logger, temporaryFolder));
            _invoker.AddAction(new FileSaveThumbprintAsCertificateAction(logger, temporaryCertificateFilePath, InfoShareSTSConfig.Path.AbsolutePath, InfoShareSTSConfig.CertificateThumbprintXPath));
            _invoker.AddAction(new FileReadAllTextAction(logger, temporaryCertificateFilePath, result => certificateContent = result));

            _invoker.AddAction(new FileGenerateFromTemplateAction(logger, 
                TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName,
                Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName),
                new Dictionary<string, string>
                {
                    {"$ishhostname", deployment.AccessHostName},
                    {"$ishcmwebappname", deployment.GetCMWebAppName()},
                    {"$ishwswebappname", deployment.GetWSWebAppName()},
                    {"$ishwscertificate", TemporarySTSConfigurationFileNames.ISHWSCertificateFileName},
                    {"$ishwscontent", certificateContent}
                }));

            if (packAdfsInvokeScript)
            {
                _invoker.AddAction(new FileGenerateFromTemplateAction(logger,
                    TemporarySTSConfigurationFileNames.ADFSInvokeTemplate,
                    Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.ADFSInvokeTemplate),
                    new Dictionary<string, string>
                    {
                        {"#!#installtool:BASEHOSTNAME#!#", deployment.AccessHostName},
                        {"#!#installtool:PROJECTSUFFIX#!#", deployment.GetSuffix()},
                        {"#!#installtool:OSUSER#!#", deployment.GetOSUser()},
                        {"#!#installtool:INFOSHAREAUTHORWEBAPPNAME#!#", deployment.GetCMWebAppName()},
                        {"#!#installtool:INFOSHAREWSWEBAPPNAME#!#", deployment.GetWSWebAppName()}
                    }));
            }

            _invoker.AddAction(new DirectoryCreateZipPackageAction(logger, temporaryFolder, packageFilePath));
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
