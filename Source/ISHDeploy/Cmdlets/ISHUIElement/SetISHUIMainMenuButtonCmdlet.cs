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
    ///		<para type="synopsis">Update or add a new MainMenuBar button.</para>
    ///		<para type="description">The Set-ISHUIMainMenuButton cmdlet updates or adds new Button definitions to Content Manager deployment.</para>
    ///		<para type="description">If Icon is not specified, the default value '~/UIFramework/events.32x32.png' is taken.</para>
    ///		<para type="description">If UserRole is not specified, the default value 'Administrator' is taken.</para>
    ///		<para type="description">If ModifiedSinceMinutesFilter is not specified, the default value '1440' is taken.</para>
    ///		<para type="description">If SelectedStatusFilter is not specified, the default value 'Recent' is taken.</para>
    ///		<para type="link">Move-ISHUIMainMenuButton</para>
    ///		<para type="link">Remove-ISHUIMainMenuButton</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHUIMainMenuButton -ISHDeployment $deployment -Label "Inbox" -Action "ShowSubMenu.asp?Menu=2" -UserRole Author, Reviewer</code>
    ///		<para>This command add/update main menu item.</para>
    ///		<para>This command sets XML definitions to MainMenuBar.
    ///			Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHUIMainMenuButton")]
    public sealed class SetISHUIMainMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Label of menu item.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Menu Label")]
        public string Label { get; set; }

        /// <summary>
        /// <para type="description">The list of users for whom the menu item is available.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Nested roles")]
        public string[] UserRole { get; set; }

        /// <summary>
        /// <para type="description">The action which occurs on click on the menu item.</para>
        /// </summary>
        [Parameter(HelpMessage = "Action to do after choosing menu")]
        public string Action { get; set; }

        /// <summary>
        /// <para type="description">The menu item identifier.</para>
        /// </summary>
        [Parameter(HelpMessage = "Unique id")]
        public string ID { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (ID == null)
            {
                ID = Label.ToUpper();
            }

            var model = new MainMenuBarItem(Label, UserRole, Action, ID);
            var setOperation = new SetUIElementOperation(Logger, ISHDeployment, model);
            setOperation.Run();
        }
    }
}
