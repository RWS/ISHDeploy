/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using ISHDeploy.Business.Operations.ISHUIEventMonitorTab;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIEventMonitorMenuBarItem
{
	/// <summary>
	///		<para type="synopsis">Removes tab from EventMonitorTab.</para>
	///		<para type="description">The Removes-ISHUIEventMonitorMenuBarItem cmdlet removes Tabs definitions from Content Manager deployment.</para>
	///		<para type="link">Set-ISHUIEventMonitorMenuBarItem</para>
	///		<para type="link">Move-ISHUIEventMonitorMenuBarItem</para>
	/// </summary>
	/// <example>
	///		<code>PS C:\>Remove-ISHUIEventMonitorMenuBarItem -ISHDeployment $deployment -Label "Translation"</code>
	///		<para>Removes definition of the tab with label "Translation".
	/// This command removes XML definitions from EventMonitor.
	/// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
	[Cmdlet(VerbsCommon.Remove, "ISHUIEventMonitorMenuBarItem")]
    public class RemoveISHUIEventMonitorMenuBarItemCmdlet : BaseHistoryEntryCmdlet
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
            var operation = new RemoveISHUIEventMonitorMenuBarItemOperation(Logger, ISHDeployment, Label);

			operation.Run();
        }
    }
}
