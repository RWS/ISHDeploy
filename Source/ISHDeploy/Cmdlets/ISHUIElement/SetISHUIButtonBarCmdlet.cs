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

using ISHDeploy.Models.UI;
using ISHDeploy.Business.Enums;
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIElement;

namespace ISHDeploy.Cmdlets.ISHUIElement
{
    /// <summary>
    /// <para type="synopsis">Create/Update button bar item.</para>
    /// <para type="description">The Set-ISHUIButtonBar cmdlet add or update button bar item.</para>
    /// <para type="link">Move-ISHUIButtonBar</para>    
    /// <para type="link">Delete-ISHUIButtonBar</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHUIButtonBar -ISHDeployment $deployment -ButtonBar DefaultSettingsButtonbar -ISHTYPE VDOCTYPEILLUSTRATION, VDOCTYPEMAP -Name "test" -OnClick "refresh();"</code>
    /// <para>This command add/update main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHUIButtonBar")]
    public sealed class SetISHUIButtonBarCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
		/// <para type="description">Type or file name correspond to Button Bar.</para>
		/// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Button bar type")]
        public ButtonBarType ButtonBar { get; set; }

        /// <summary>
        /// <para type="description">ISHType is a list of the card type.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Array of enum/object -in xml filed name is CARDTYPE")]
        public CardType[] ISHTYPE { get; set; }

        /// <summary>
        /// <para type="description">Name of Button Bar.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Name of button bar")]
        public string Name { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Icon file name")]
        public string Icon { get; set; }

        /// <summary>
        /// <para type="description">Javascript or asp page need to be invoken after click.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "OnClick javascript function name")]
        public string OnClick { get; set; }
        
        /// <summary>
        /// <para type="description">Check access.</para>
        /// </summary>
        [Parameter(HelpMessage = "files for settings")]
        public string Checkaccess { get; set; } = "N";

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var model = new ButtonBarItem(ButtonBar, Name, ISHTYPE, Icon, OnClick, Checkaccess);
            var setOperation = new SetUIElementOperation(Logger, ISHDeployment, model);
            setOperation.Run();
        }
    }
}
