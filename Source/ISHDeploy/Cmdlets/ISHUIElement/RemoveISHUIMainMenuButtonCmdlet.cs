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

using ISHDeploy.Business.Operations.ISHUIElement;
using ISHDeploy.Models.UI;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIElement
{
    /// <summary>
	///		<para type="synopsis">Removes button from MainMenuBar.</para>
	///		<para type="description">The Removes-ISHUIMainMenuButton cmdlet removes Buttons definitions from Content Manager deployment.</para>
	///		<para type="link">Set-ISHUIMainMenuButton</para>
	///		<para type="link">Move-ISHUIMainMenuButton</para>
	/// </summary>
	/// <example>
	///		<code>PS C:\>Remove-ISHUIMainMenuButton -ISHDeployment $deployment -Label "Translation"</code>
	///		<para>Removes definition of the button with label "Translation".
	/// This command removes XML definitions from EventMonitor.
	/// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
	///		</para>
	/// </example>
    [Cmdlet(VerbsCommon.Remove, "ISHUIMainMenuButton")]
    public sealed class RemoveISHUIMainMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Label of menu item.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Menu Label")]
        public string Label { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var model = new MainMenuBarItem(Label);
            var operation = new RemoveUIElementOperation(Logger, ISHDeployment, model);
            operation.Run();
        }
    }

}
