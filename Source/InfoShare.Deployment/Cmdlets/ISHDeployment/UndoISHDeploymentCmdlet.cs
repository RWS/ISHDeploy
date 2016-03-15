using System.Management.Automation;
using InfoShare.Deployment.Business.Operations.ISHDeployment;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Reverts all customization done by cmdlets back to original state for specific Content Manager deployment.</para>
    /// <para type="description">The Undo-ISHDeployment cmdlet reverts all customization done by cmdlets back to original state for specific Content Manager deployment.</para>
    /// <para type="description">Original state means the state of the system when it was installed and no customization was made.</para>
    /// <para type="link">Clear-ISHDeploymentHistory</para>
    /// <para type="link">Get-ISHDeployment</para>
    /// <para type="link">Get-ISHDeploymentHistory</para>
    /// </summary>
    /// <example>
    /// <para>Revert Content Manager to original state:</para>
    /// <code>Undo-ISHDeployment -ISHDeployment $deployment</code>
    /// <para>Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// <para></para>
    /// </example>
    [Cmdlet(VerbsCommon.Undo, "ISHDeployment")]
    public class UndoISHDeploymentCmdlet : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
		public Models.ISHDeployment ISHDeployment { get; set; }

		/// <summary>
		/// Executes revert changes cmdLet
		/// </summary>
        public override void ExecuteCmdlet()
		{
			var cmdSet = new UndoISHDeploymentOperation(Logger, ISHDeployment);
			cmdSet.Run();
		}
    }
}
