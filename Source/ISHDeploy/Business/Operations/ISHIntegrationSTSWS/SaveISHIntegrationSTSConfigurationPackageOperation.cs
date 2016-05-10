using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Data.Actions.Template;
using ISHDeploy.Data.Managers;
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
        /// <param name="paths">The instance of <see cref="ISHPaths" />.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="packAdfsInvokeScript">if set to <c>true</c> the add ADFS script invocation into package.</param>
        public SaveISHIntegrationSTSConfigurationPackageOperation(ILogger logger, ISHPaths paths, string fileName, bool packAdfsInvokeScript = false)
        {
            _invoker = new ActionInvoker(logger, "Saving STS integration configuration");

            var packageFileName = Regex.Replace(fileName, @"(?<FileName>.*)(.zip)$", "${FileName}") + ".zip";
            var packageFilePath = Path.Combine(paths.PackagesFolderPath, packageFileName);
            var certificateFilePath = Path.Combine(paths.PackagesFolderPath, "ishws.cer");
            var docFilePath = Path.Combine(paths.PackagesFolderPath, TemplateManager.CMSecurityTokenServiceTemplate);
            var adfsInvokeScriptPath = Path.Combine(paths.PackagesFolderPath, TemplateManager.ADFSInvokeTemplate);

            _invoker.AddAction(new SaveCertificateAction(logger, certificateFilePath, InfoShareSTSConfig.Path.AbsolutePath, InfoShareSTSConfig.CertificateThumbprintXPath));
            _invoker.AddAction(new SaveCMSecurityTokenServiceAction(logger, docFilePath, certificateFilePath, paths.AccessHostName, paths.CMWebAppName, paths.WSWebAppName));
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
