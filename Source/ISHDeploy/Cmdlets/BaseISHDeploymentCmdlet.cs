using System;
using System.Management.Automation;
using ISHDeploy.Business.Operations;
using ISHDeploy.Validators;
using ISHDeploy.Business.Operations.ISHDeployment;

namespace ISHDeploy.Cmdlets
{
    /// <summary>
    /// Provides base functionality for all cmdlets that use an instance of the Content Manager deployment.
    /// </summary>
    public abstract class BaseISHDeploymentCmdlet : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        [ValidateDeploymentVersion]
        public Models.ISHDeployment ISHDeployment { get; set; }

        public Models.ISHDeploymentExtended ISHDeploymentExtended { get; set; }

        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            ISHDeploymentExtended = new GetISHDeploymentOperation(Logger, ISHDeployment.Name).Run();
            OperationPaths.Initialize(ISHDeploymentExtended);
        }

    }
}
