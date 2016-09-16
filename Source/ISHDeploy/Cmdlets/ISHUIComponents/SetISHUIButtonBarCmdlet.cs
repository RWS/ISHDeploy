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

using ISHDeploy.Business.Operations.ISHUIOperation;
using ISHDeploy.Models.UI;
using ISHDeploy.Business.Enums;
using System.Management.Automation;

namespace ISHDeploy.Cmdlets.ISHUIComponents
{
    /// <summary>
    /// <para type="synopsis">Create/Update button bar item.</para>
    /// <para type="description">The Set-ISHUIButtonBar cmdlet add or update button bar item.</para>
    /// <para type="link">Move-ISHUIButtonBar</para>    
    /// <para type="link">Delete-ISHUIButtonBar</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHUIMainMenuButton -ISHDeployment $deployment </code>
    /// <para>This command add/update main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHUIButtonBar")]
    public sealed class SetISHUIButtonBarCmdlet : BaseHistoryEntryCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Button bar type")]
        public ButtonBarType ButtonBar { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Array of enum/object -in xml filed name is CARDTYPE")]
        public CardType[] ISHTYPE { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Name of button bar")]
        public string Name { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Icon file name")]
        public string Icon { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "OnClick javascript function name")]
        public string OnClick { get; set; }

        [Parameter(HelpMessage = "files for settings")]
        public string Checkaccess { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var model = new ButtonBarModel(ButtonBar, ISHTYPE, Name, Icon, OnClick, Checkaccess);
            var setOperation = new SetUIOperation(Logger, ISHDeployment, model);
            setOperation.Run();
        }
    }
}
