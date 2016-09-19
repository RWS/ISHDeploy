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

namespace ISHDeploy.Cmdlets.ISHUIComponents
{
    /// <summary>
    /// <para type="synopsis">Create/Update search menu item.</para>
    /// <para type="description">The Set-ISHUISearchMenuButton cmdlet add or update search menu item.</para>
    /// <para type="link">Move-ISHUISearchMenuButton</para>    
    /// <para type="link">Remove-ISHUISearchMenuButton</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHUISearchMenuButton -ISHDeployment $deployment -Label "Search" -SearchType Publication -Title "Publications" -UserRole Author, Reviewer</code>
    /// <para>This command add/update main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHUISearchMenuButton")]
    public sealed class SetISHUISearchMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Menu Label")]
        public string Label { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Nested roles")]
        public string[] UserRole { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Action to do after choosing menu")]
        public SearchType SearchType { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Action to do after choosing menu")]
        public string Title { get; set; }

        [Parameter(HelpMessage = "Icon for a menu")]
        public string Icon { get; set; }

        [Parameter(HelpMessage = "Action to do after choosing menu")]
        public string SearchXML { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            string searchType;
            if (SearchType == SearchType.Publication)
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
