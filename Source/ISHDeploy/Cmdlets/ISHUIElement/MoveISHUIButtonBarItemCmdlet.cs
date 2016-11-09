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
    /// <para type="description">The Move-ISHUIButtonBarItem cmdlet remove exist menu item.</para>
    /// <para type="link">Remove-ISHUIButtonBarItem</para>    
    /// <para type="link">Set-ISHUIButtonBarItem</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Move-ISHUIButtonBarItem -ISHDeployment $deployment -Logical -First -Name "test"</code>
    /// <para>This command move main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Move, "ISHUIButtonBarItem")]
    [AdministratorRights]
    public sealed class MoveISHUIButtonBarItemCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Menu item move position.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Logical After")]
        [Parameter(Mandatory = true, ParameterSetName = "Version After")]
        [Parameter(Mandatory = true, ParameterSetName = "Language After")]
        public string After { get; set; }

        /// <summary>
        /// <para type="description">Menu item move to the first position.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Logical First")]
        [Parameter(Mandatory = true, ParameterSetName = "Version First")]
        [Parameter(Mandatory = true, ParameterSetName = "Language First")]
        public SwitchParameter First { get; set; }

        /// <summary>
        /// <para type="description">Menu item move to the last position.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Logical Last")]
        [Parameter(Mandatory = true, ParameterSetName = "Version First")]
        [Parameter(Mandatory = true, ParameterSetName = "Language First")]
        public SwitchParameter Last { get; set; }

        /// <summary>
        /// <para type="description">Type "Logical" especially for FolderButtonbar.xml.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Logical Last")]
        [Parameter(Mandatory = true, ParameterSetName = "Logical First")]
        [Parameter(Mandatory = true, ParameterSetName = "Logical After")]
        public SwitchParameter Logical { get; set; }

        /// <summary>
        /// <para type="description">Type "Version" especially for LanguageDocumentButtonbar.xml.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Version Last")]
        [Parameter(Mandatory = true, ParameterSetName = "Version First")]
        [Parameter(Mandatory = true, ParameterSetName = "Version After")]
        public SwitchParameter Version { get; set; }

        /// <summary>
        /// <para type="description">Type "Language" especially for TopDocumentButtonbar.xml.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Language Last")]
        [Parameter(Mandatory = true, ParameterSetName = "Language First")]
        [Parameter(Mandatory = true, ParameterSetName = "Language After")]
        public SwitchParameter Language { get; set; }

        /// <summary>
        /// <para type="description">Name of Button Bar.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Button bar name")]
        public string Name { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            UIElementMoveDirection direction = UIElementMoveDirection.Last;

            if (ParameterSetName.Contains("Last"))
                direction = UIElementMoveDirection.Last;
            if (ParameterSetName.Contains("First"))
                direction = UIElementMoveDirection.First;
            if (ParameterSetName.Contains("After"))
                direction = UIElementMoveDirection.After;
            if (!(ParameterSetName.Contains("Last") || ParameterSetName.Contains("First") || ParameterSetName.Contains("After")))
                throw new System.ArgumentException($"Operation type in {nameof(MoveISHUIButtonBarItemCmdlet)} should be defined.");

            string buttonBarFile = null;
            if (ParameterSetName.Contains("Logical"))
                buttonBarFile = "FolderButtonbar.xml";
            if (ParameterSetName.Contains("Version"))
                buttonBarFile = "LanguageDocumentButtonbar.xml";
            if (ParameterSetName.Contains("Language"))
                buttonBarFile = "TopDocumentButtonbar.xml";


            var model = new ButtonBarItem(buttonBarFile, Name);
            var operation = new MoveUIElementOperation(Logger, ISHDeployment, model, direction, After);
            operation.Run();
        }
    }
}
