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
using ISHDeploy.Business.Operations.ISHUIElement;
using ISHDeploy.Models.UI;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIElement
{
    /// <summary>
    /// <para type="synopsis">Move main menu item.</para>
    /// <para type="description">The Move-ISHUIButtonBar cmdlet remove exist menu item.</para>
    /// <para type="link">Remove-ISHUIButtonBar</para>    
    /// <para type="link">Set-ISHUIButtonBar</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Move-ISHUIButtonBar -ISHDeployment $deployment -Name "Test"</code>
    /// <para>This command move main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Move, "ISHUIButtonBar")]
    public sealed class MoveISHUIButtonBarCmdlet : BaseHistoryEntryCmdlet
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
		/// <para type="description">Menu item move to the last position.</para>
		/// </summary>
		[Parameter(Mandatory = false, HelpMessage = "Menu item move to the last position", ParameterSetName = "Last")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter Last { get; set; }

        /// <summary>
        /// <para type="description">Menu item move to the first position.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Menu item move to the first position", ParameterSetName = "First")]
        [ValidateNotNullOrEmpty]
        public SwitchParameter First { get; set; }

        /// <summary>
        /// <para type="description">Menu item move position.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Menu item move position", ParameterSetName = "After")]
        [ValidateNotNullOrEmpty]
        public string After { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            UIElementMoveDirection direction;
            switch (ParameterSetName)
            {
                case "Last":
                    direction = UIElementMoveDirection.Last;
                    break;
                case "First":
                    direction = UIElementMoveDirection.First;
                    break;
                case "After":
                    direction = UIElementMoveDirection.After;
                    break;
                default:
                    throw new System.ArgumentException($"Operation type in {nameof(MoveISHUIButtonBarCmdlet)} should be defined.");
            }

            var model = new ButtonBarItem(ButtonBar, Name);
            var operation = new MoveUIElementOperation(Logger, ISHDeployment, model, direction, After);
            operation.Run();
        }
    }
}
