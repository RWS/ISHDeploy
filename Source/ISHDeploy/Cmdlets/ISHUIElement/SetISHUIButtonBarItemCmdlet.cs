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
using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHUIElement;
using System;
using System.Collections.Generic;
using ISHDeploy.Business.Enums;

namespace ISHDeploy.Cmdlets.ISHUIElement
{
    /// <summary>
    /// <para type="synopsis">Create/Update button bar item.</para>
    /// <para type="description">The Set-ISHUIButtonBarItem cmdlet add or update button bar item.</para>
    /// <para type="link">Move-ISHUIButtonBarItem</para>    
    /// <para type="link">Remove-ISHUIButtonBarItem</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Set-ISHUIButtonBarItem -ISHDeployment $deployment -Logical -ISHType ISHIllustration, ISHMasterDoc -Name "test" -Action "refresh();" -Icon "~/UIFramework/new-tab.job.32x32.png"</code>
    /// <para>This command add/update main menu item.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHUIButtonBarItem")]
    public sealed class SetISHUIButtonBarItemCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Name of the button.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Name of button bar")]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Icon for the button.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Icon file name")]
        public string Icon { get; set; }

        /// <summary>
        /// <para type="description">Javascript or asp page need to be invoken after click.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "OnClick javascript function name")]
        public string Action { get; set; }

        /// <summary>
        /// <para type="description">Check access.</para>
        /// </summary>
        [Parameter(HelpMessage = "Files for settings")]
        public string Checkaccess { get; set; }

        /// <summary>
        /// <para type="description">Hide text.</para>
        /// </summary>
        [Parameter(HelpMessage = "Hides text")]
        public SwitchParameter HideText { get; set; }

        /// <summary>
        /// <para type="description">Type "Logical" especially for FolderButtonbar.xml.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Logical", HelpMessage = "Using for FolderButtonbar.xml")]
        public SwitchParameter Logical { get; set; }

        /// <summary>
        /// <para type="description">Type "Version" especially for LanguageDocumentButtonbar.xml.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Version", HelpMessage = "Using for LanguageDocumentButtonbar.xml")]
        public SwitchParameter Version { get; set; }

        /// <summary>
        /// <para type="description">Type "Language" especially for TopDocumentButtonbar.xml.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Language", HelpMessage = "Using for TopDocumentButtonbar.xml")]
        public SwitchParameter Language { get; set; }

        /// <summary>
        /// <para type="description">ISHType is a list of names for correspond card type.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Logical", HelpMessage = "Array of enum/object for correspond in xml CARDTYPE")]
        [Parameter(Mandatory = true, ParameterSetName = "Version", HelpMessage = "Array of enum/object for correspond in xml CARDTYPE")]
        [Parameter(Mandatory = true, ParameterSetName = "Language", HelpMessage = "Array of enum/object for correspond in xml CARDTYPE")]
        public CardType[] ISHType { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            string buttonBarFile = null;
            string[] cards = null;
            switch (ParameterSetName)
            {
                case "Logical":
                    buttonBarFile = "FolderButtonbar.xml";
                    cards = GetCardsArray(ISHType, logicalDictionary);
                    if (Checkaccess == null)
                        Checkaccess = "N";
                    break;
                case "Version":
                    buttonBarFile = "LanguageDocumentButtonbar.xml";
                    cards = GetCardsArray(ISHType, versionDictionary);
                    if (Checkaccess == null)
                        Checkaccess = "N";
                    break;
                case "Language":
                    buttonBarFile = "TopDocumentButtonbar.xml";
                    cards = GetCardsArray(ISHType, versionDictionary); // is the same as in Version
                    if (Checkaccess == null)
                        Checkaccess = "N";
                    break;
                default:
                    throw new ArgumentException($"Unknown parameter {ParameterSetName}");
            }

            var model = new ButtonBarItem(buttonBarFile, Name, cards, Icon, Action, Checkaccess, HideText.IsPresent);
            var setOperation = new SetUIElementOperation(Logger, ISHDeployment, model);
            setOperation.Run();
        }

        private Dictionary<string, string> logicalDictionary = new Dictionary<string, string>(){
                        {"ISHIllustration", "VDOCTYPEILLUSTRATION"},
                        {"ISHModule", "VDOCTYPEMAP"},
                        {"ISHMasterDoc", "VDOCTYPEMASTER"},
                        {"ISHTemplate","VDOCTYPETEMPLATE"},
                        {"ISHLibrary","VDOCTYPELIB"},
                        {"ISHReference","VDOCTYPEREFERENCE"},
                        {"ISHQuery","VDOCTYPEQUERY"},
                        {"ISHPublication","VDOCTYPEPUBLICATION"}};
        private Dictionary<string, string> versionDictionary = new Dictionary<string, string>(){
                        {"ISHModule","CTMAP"},
                        {"ISHMasterDoc","CTMASTER"},
                        {"ISHTemplate","CTTEMPLATE"},
                        {"ISHIllustration","CTIMG"},
                        {"ISHLibrary","CTLIB"},
                        {"ISHPublication","CTPUBLICATION"}};

        private string[] GetCardsArray(CardType[] ishTypes, Dictionary<string, string> dict)
        {
            List<string> cards = new List<string>();
            foreach (CardType type in ishTypes)
            {
                string value;
                if (!dict.TryGetValue(type.ToString(), out value))
                {
                    throw new ArgumentException($"Cannot find card type for {type}.");
                }
                cards.Add(value);
            }
            return cards.ToArray();
        }
    }
}
