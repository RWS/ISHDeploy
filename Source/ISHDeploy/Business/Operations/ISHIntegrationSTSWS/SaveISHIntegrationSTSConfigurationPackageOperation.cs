using System.Collections.Generic;
using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.Certificate;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationSTSWS
{
    /// <summary>
    /// Saves current STS integration configuration to zip package.
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class SaveISHIntegrationSTSConfigurationPackageOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveISHIntegrationSTSConfigurationPackageOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="packAdfsInvokeScript">if set to <c>true</c> the add ADFS script invocation into package.</param>
        public SaveISHIntegrationSTSConfigurationPackageOperation(ILogger logger, Models.ISHDeployment ishDeployment, string fileName, bool packAdfsInvokeScript = false) :
            base (logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Saving STS integration configuration");

            var packageFilePath = Path.Combine(FoldersPaths.PackagesFolderPath, fileName);
            var temporaryFolder = Path.Combine(Path.GetTempPath(), fileName);
            var temporaryCertificateFilePath = Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.ISHWSCertificateFileName);

            var stsConfigParams = new Dictionary<string, string>
                    {
                        {"$ishhostname", ISHDeploymentInternal.AccessHostName},
                        {"$ishcmwebappname", ISHDeploymentInternal.WebAppNameCM},
                        {"$ishwswebappname", ISHDeploymentInternal.WebAppNameWS},
                        {"$ishwscertificate", TemporarySTSConfigurationFileNames.ISHWSCertificateFileName},
                        {"$ishwscontent", string.Empty}
                    };

            _invoker.AddAction(new DirectoryCreateAction(logger, temporaryFolder));
            _invoker.AddAction(new SaveThumbprintAsCertificateAction(logger, temporaryCertificateFilePath, InfoShareWSWebConfig.Path.AbsolutePath, InfoShareWSWebConfig.CertificateThumbprintXPath));
            _invoker.AddAction(new FileReadAllTextAction(logger, temporaryCertificateFilePath, result => stsConfigParams["$ishwscontent"] = result));

            _invoker.AddAction(new FileGenerateFromTemplateAction(logger, 
                TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName,
                Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.CMSecurityTokenServiceTemplateFileName),
                stsConfigParams));

            if (packAdfsInvokeScript)
            {
                _invoker.AddAction(new FileGenerateFromTemplateAction(logger,
                    TemporarySTSConfigurationFileNames.ADFSInvokeTemplate,
                    Path.Combine(temporaryFolder, TemporarySTSConfigurationFileNames.ADFSInvokeTemplate),
                    new Dictionary<string, string>
                    {
                        {"#!#installtool:BASEHOSTNAME#!#", ISHDeploymentInternal.AccessHostName},
                        {"#!#installtool:PROJECTSUFFIX#!#", ISHDeploymentInternal.ProjectSuffix},
                        {"#!#installtool:OSUSER#!#", ISHDeploymentInternal.OSUser},
                        {"#!#installtool:INFOSHAREAUTHORWEBAPPNAME#!#", ISHDeploymentInternal.WebAppNameCM},
                        {"#!#installtool:INFOSHAREWSWEBAPPNAME#!#", ISHDeploymentInternal.WebAppNameWS}
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
