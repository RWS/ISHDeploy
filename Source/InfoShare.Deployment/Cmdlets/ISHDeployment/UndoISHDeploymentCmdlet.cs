using System.Management.Automation;
using InfoShare.Deployment.Business.Operations.ISHDeployment;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Undo, "ISHDeployment")]
    public class UndoISHDeploymentCmdlet : BaseCmdlet
    {
        /// <summary>
        /// Deployment prefix to identify deployment to be rolled back
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, HelpMessage = "Suffix of the already deployed Content Manager instance")]
        [Alias("Suffix")]
        public string Deployment { get; set; }

		/// <summary>
		/// Executes revert changes cmdLet
		/// </summary>
        public override void ExecuteCmdlet()
        {
			var cmdSet = new UndoISHDeploymentOperation(Logger, Deployment);
			cmdSet.Run();
		}
    }
}
