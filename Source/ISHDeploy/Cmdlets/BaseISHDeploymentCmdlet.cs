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
        public Models.ISHDeployment ISHDeployment {
            set
            {
                ISHDeploymentExtended = new GetISHDeploymentExtendedOperation(Logger, value.Name).Run();
                OperationPaths.Initialize(ISHDeploymentExtended);
            }
        }

        /// <summary>
        /// <para type="description">Extended description of the instance of the Content Manager deployment.</para>
        /// </summary>
        protected Models.ISHDeploymentExtended ISHDeploymentExtended { get; private set; }
    }
}
