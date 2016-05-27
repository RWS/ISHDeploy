using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHDeployment;

namespace ISHDeploy.Cmdlets.ISHDeployment
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
    /// <code>PS C:\>Undo-ISHDeployment -ISHDeployment $deployment</code>
    /// <para>This command reverts Content Manager to original state.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Undo, "ISHDeployment")]
    public class UndoISHDeploymentCmdlet : BaseISHDeploymentCmdlet
    {
		/// <summary>
		/// Executes revert changes cmdLet
		/// </summary>
        public override void ExecuteCmdlet()
		{
			var cmdSet = new UndoISHDeploymentOperation(Logger, ISHDeploymentExtended);
			cmdSet.Run();
		}
    }
}
