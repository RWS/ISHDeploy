using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using ISHDeploy.Business.Invokers;
using ISHDeploy.Cmdlets.ISHIntegrationDB;
using ISHDeploy.Data.Actions.Directory;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Business.Operations.ISHIntegrationDB
{
    /// <summary>
    /// Generates SQL Server script that grants necessary permissions.
    /// </summary>
    /// <seealso cref="BasePathsOperation" />
    /// <seealso cref="IOperation" />
    public class SaveISHIntegrationDBSTSSQLServerConfigurationOperation : BasePathsOperation, IOperation
    {
        /// <summary>
        /// The actions invoker.
        /// </summary>
        private readonly IActionInvoker _invoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveISHIntegrationDBSTSSQLServerConfigurationOperation" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ishDeployment">The instance of the deployment.</param>
        /// <param name="fileName">Name of the output file.</param>
        /// <param name="type">The output file type.</param>
        public SaveISHIntegrationDBSTSSQLServerConfigurationOperation(ILogger logger, Models.ISHDeployment ishDeployment, string fileName, SaveISHIntegrationDBSTSSQLServerConfigurationCmdlet.OutputType type) :
            base (logger, ishDeployment)
        {
            _invoker = new ActionInvoker(logger, "Saving STS integration configuration");

            string templateFile;
            
            switch (type)
            {
                case SaveISHIntegrationDBSTSSQLServerConfigurationCmdlet.OutputType.PS1:
                    templateFile = TemporaryDBConfigurationFileNames.GrantComputerAccountPermissionsPSTemplate;
                    break;
                case SaveISHIntegrationDBSTSSQLServerConfigurationCmdlet.OutputType.SQL:
                default:
                    templateFile = TemporaryDBConfigurationFileNames.GrantComputerAccountPermissionsSQLTemplate;
                    break;
            }

            _invoker.AddAction(new DirectoryEnsureExistsAction(logger, FoldersPaths.PackagesFolderPath));

            using (OleDbConnection builder = new OleDbConnection(ISHDeploymentInternal.ConnectString))
            {
                _invoker.AddAction(new FileGenerateFromTemplateAction(logger,
                    templateFile,
                    Path.Combine(FoldersPaths.PackagesFolderPath, fileName),
                    new Dictionary<string, string>
                    {
                        {"$OSUSER$", ISHDeploymentInternal.OSUser},
                        {"$DATABASE$", builder.Database},
                        {"$DATASOURCE$", builder.DataSource}
                    }));
            }
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
