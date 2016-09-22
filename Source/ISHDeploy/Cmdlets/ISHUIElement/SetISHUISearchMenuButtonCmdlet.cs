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
    /// <para type="synopsis">Create/Update search menu item.</para>
    /// <para type="description">The Set-ISHUISearchMenuButton cmdlet add or update search menu item.</para>
    /// <para type="link">Move-ISHUISearchMenuButton</para>    
    /// <para type="link">Remove-ISHUISearchMenuButton</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHUISearchMenuButton -ISHDeployment $deployment -Label "Search" -SearchType Publication -Title "Publications" -UserRole Author, Reviewer</code>
    /// <para>This command add/update search menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHUISearchMenuButton")]
    public sealed class SetISHUISearchMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Label of menu item.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Menu Label.")]
        public string Label { get; set; }

        /// <summary>
        /// <para type="description">The list of users for whom the menu item is available.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Nested roles.")]
        public string[] UserRole { get; set; }

        /// <summary>
        /// <para type="description">Search type will choose asp page SearchFrame.asp or SearchNewPublications.asp .</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Search type to choose asp page.")]
        public SearchType SearchType { get; set; }

        /// <summary>
        /// <para type="description">Title for a menu.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Title for a menu.")]
        public string Title { get; set; }

        /// <summary>
        /// <para type="description">Icon for a menu.</para>
        /// </summary>
        [Parameter(HelpMessage = "Icon for a menu.")]
        public string Icon { get; set; }

        /// <summary>
        /// <para type="description">SearchXML parameter in action. By default is the same as SearchType.</para>
        /// </summary>
        [Parameter(HelpMessage = "Action to do after choosing menu.")]
        public string SearchXML { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            string searchType;
            if (SearchType == SearchType.Frame)
            {
                searchType = "SearchFrame";
            }
            else
            {
                searchType = "SearchNewPublications";
            }

            if (SearchXML == null)
            {
                SearchXML = searchType;
            }

            string action = $"{searchType}.asp?SearchXml={SearchXML}&amp;Title={Title}";
            var model = new SearchMenuItem(Label, UserRole, Icon, action);
            var setOperation = new SetUIElementOperation(Logger, ISHDeployment, model);
            setOperation.Run();
        }
    }
}
