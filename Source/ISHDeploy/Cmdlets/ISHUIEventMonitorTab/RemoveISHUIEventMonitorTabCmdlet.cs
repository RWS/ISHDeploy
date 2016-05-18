using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIEventMonitorTab;

namespace ISHDeploy.Cmdlets.ISHUIEventMonitorTab
{
	/// <summary>
	///		<para type="synopsis">Removes tab from EventMonitorTab.</para>
	///		<para type="description">The Removes-ISHUIEventMonitorTab cmdlet removes Tabs definitions from Content Manager deployment.</para>
	///		<para type="link">Set-ISHUIEventMonitorTab</para>
	///		<para type="link">Move-ISHUIEventMonitorTab</para>
	/// </summary>
	/// <example>
	///		<code>PS C:\>Remove-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "Translation"</code>
	///		<para>Removes definition of the tab with label "Translation".</para>
	///		<para>This command removes XML definitions from EventMonitor.
	///			Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	[Cmdlet(VerbsCommon.Remove, "ISHUIEventMonitorTab")]
    public class RemoveISHUIEventMonitorTabCmdlet : BaseHistoryEntryCmdlet
    {
		/// <summary>
		/// <para type="description">Label of menu item.</para>
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Label of menu item")]
		[ValidateNotNullOrEmpty]
		public string Label { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new RemoveISHUIEventMonitorTabOperation(Logger, Label);

			operation.Run();
        }
    }
}
