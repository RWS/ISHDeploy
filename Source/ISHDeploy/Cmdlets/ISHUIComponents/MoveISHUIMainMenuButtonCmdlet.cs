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
using ISHDeploy.Business.Operations.ISHUIOperation;
using ISHDeploy.Models.UI;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIComponents
{
    /// <summary>
    /// <para type="synopsis">Remove main menu item.</para>
    /// <para type="description">The Remove-ISHUIMainMenuButton cmdlet remove exist menu item.</para>
    /// <para type="link">Move-ISHUIMainMenuButton</para>    
    /// <para type="link">Set-ISHUIMainMenuButton</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Remove-ISHUIMainMenuButton -ISHDeployment $deployment -Label "Inbox2"</code>
    /// <para>This command remove main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Move, "ISHUIMainMenuButton")]
    public sealed class MoveISHUIMainMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Menu Label")]
        public string Label { get; set; }
        
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
            OperationType operationType;
            switch (ParameterSetName)
            {
                case "Last":
                    operationType = OperationType.Last;
                    break;
                case "First":
                    operationType = OperationType.First;
                    break;
                case "After":
                    operationType = OperationType.After;
                    break;
                default:
                    throw new System.ArgumentException($"Operation type in {nameof(MoveISHUIMainMenuButtonCmdlet)} should be defined.");
            }

            var model = new MainMenuBarItemModel(Label);
            var operation = new MoveUIOperation(Logger, ISHDeployment, model, operationType, After);
            operation.Run();
        }
    }

}
