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

using ISHDeploy.Data.Managers;
using System.Management.Automation;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ISHDeploy.Cmdlets.ISHUIComponents
{
    /// <summary>
    /// <para type="synopsis">Disables Content Editor for Content Manager deployment.</para>
    /// <para type="description">The Disable-ISHUIContentEditor cmdlet disables Content Editor for Content Manager deployment.</para>
    /// <para type="link">Enable-ISHUIContentEditor</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHUIMainMenuButton -ISHDeployment $deployment -Label "Inbox2" -Action "ShowSubMenu.asp?Menu=2" -UserRole Author, Reviewer</code>
    /// <para>This command disables Content Editor.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHUIMainMenuButton")]
    public sealed class SetISHUIMainMenuButtonCmdlet : BaseHistoryEntryCmdlet
    {
        private XElement element = new XElement("menuitem");

        private string _label;
        [Parameter(Mandatory = true, HelpMessage = "Menu Label")]
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                element.Add(new XAttribute("label", value));
                _label = value;
            }
        }

        private string[] _userRole;
        [Parameter(Mandatory = true, HelpMessage = "Nested roles")]
        public string[] UserRole
        {
            get { return _userRole; }
            set
            {
                foreach (string role in value)
                {
                    element.Add(new XElement("userrole", role));
                }
                _userRole = value;
            }
        }

        private string _action;
        [Parameter(HelpMessage = "Action to do after choosing menu")]
        public string Action
        {
            get { return _action; }
            set
            {
                element.Add(new XAttribute("action", value));
                _action = value;
            }
        }

        private string _id;
        [Parameter(HelpMessage = "Unique id")]
        public string ID
        {
            get { return _id; }
            set
            {
                element.Add(new XAttribute("id", value));
                _id = value;
            }
        }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (ID == null) {
                element.Add(new XAttribute("id", Label.ToUpper()));
            }
            var t = element.ToString();

            //var operation = new Set(Logger, ISHDeployment, element, "mainmenubar");
            new XmlConfigManager(Logger).InsertBeforeNode(@"C:\InfoShare\Web\Author\ASP\XSL\MainMenuBar.xml", "./mainmenubar/menuitem", element.ToString());
        }
    }

    /*
    [Cmdlet(VerbsCommon.Move, "ISHUIContentEditor")]
    public sealed class MoveISHUIMainMenuButtonCmdlet : BaseHistoryEntryCmdlet
    { }
    [Cmdlet(VerbsLifecycle.Delete, "ISHUIContentEditor")]
    public sealed class DeleteISHUIMainMenuButtonCmdlet : BaseHistoryEntryCmdlet
    { }*/

}
