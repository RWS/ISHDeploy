using System.Management.Automation;
using InfoShare.Deployment.Business.Operations.ISHDeployment;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Undo, "ISHDeployment")]
    public class UndoISHDeploymentCmdlet : BaseCmdlet
    {
		/// <summary>
		/// Ish deployment parameter, to identify deployment to be rolled back
		/// </summary>
		[Parameter(Mandatory = false, Position = 0, HelpMessage = "Already deployed Content Manager instance, to be rolled back")]
		[Alias("proj")]
		[ValidateNotNull]
		public Models.ISHDeployment ISHDeployment { get; set; }

		/// <summary>
		/// Executes revert changes cmdLet
		/// </summary>
        public override void ExecuteCmdlet()
		{
			Models.ISHDeployment deployment = ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment;
			var cmdSet = new UndoISHDeploymentOperation(Logger, deployment);
			cmdSet.Run();
		}
    }
}
