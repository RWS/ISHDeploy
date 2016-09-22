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

using ISHDeploy.Business.Enums;
using ISHDeploy.Models.UI;
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIElement;

namespace ISHDeploy.Cmdlets.ISHUIElement
{
    /// <summary>
    /// <para type="synopsis">Remove main menu item.</para>
    /// <para type="description">The Remove-ISHUIButtonBar cmdlet remove exist menu item.</para>
    /// <para type="link">Move-ISHUIButtonBar</para>    
    /// <para type="link">Set-ISHUIButtonBar</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Remove-ISHUIButtonBar -ISHDeployment $deployment -Label "Inbox2"</code>
    /// <para>This command remove main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "ISHUIButtonBar")]
    public sealed class RemoveISHUIButtonBarCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
		/// <para type="description">Name of Button Bar.</para>
		/// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Button bar name")]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Type or file name correspond to Button Bar.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Button bar type")]
        public ButtonBarType ButtonBar { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var model = new ButtonBarItem(ButtonBar, Name);
            var operation = new RemoveUIElementOperation(Logger, ISHDeployment, model);
            operation.Run();
        }
    }
}
