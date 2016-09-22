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
using System.Xml.Serialization;

namespace ISHDeploy.Models.UI
{
    [XmlRoot("BUTTON", Namespace = "")]
    public class ButtonBarItem : BaseUIElement
    {
        [XmlAttribute("CHECKACCESS")]
        public string CheckAccess { set; get; }

        [XmlElement("CARDTYPE")]
        public CardType[] CardTypes { set; get; }

        [XmlElement("INPUT")]
        public Input Input { set; get; }

        private ButtonBarItem()
        {

        }

        public ButtonBarItem(ButtonBarType buttonBar, string name, CardType[] ishtype = null, string icon = null, string onClick = null, string checkaccess = null)
        {
            RelativeFilePath = $@"Author\ASP\XSL\{buttonBar}.xml";
            NameOfRootElement = "BUTTONBAR";
            NameOfItem = "BUTTON";

            Input = new Input();
            Input.Value = name;
            Input.Name = name;
            Input.Icon = icon;
            Input.OnClick = onClick;
            CardTypes = ishtype;
            CheckAccess = checkaccess;
            XPathFormat = "BUTTONBAR/BUTTON/INPUT[@NAME='{0}']";
            XPath = string.Format(XPathFormat, name);

            //for default card type list
            if (ishtype == null) 
            { 
                switch (buttonBar)
                {
                    case ButtonBarType.CategoryMasterButtonbar:
                        CardTypes = new [] { CardType.VDOCTYPEILLUSTRATION, CardType.VDOCTYPEMAP, CardType.VDOCTYPEMASTER };
                        break;
                    case ButtonBarType.DefaultSettingsButtonbar:
                        break;
                    case ButtonBarType.DetailButtonbar:
                        break;
                    case ButtonBarType.EventMonitorButtonbar:
                        break;
                    case ButtonBarType.EventMonitorDetailButtonbar:
                        break;
                    case ButtonBarType.FolderButtonbar:
                        break;
                    case ButtonBarType.InboxButtonBar:
                        break;
                    case ButtonBarType.LanguageDocumentButtonbar:
                        break;
                    case ButtonBarType.OutputFormatButtonbar:
                        break;
                    case ButtonBarType.RevisionsButtonbar:
                        break;
                    case ButtonBarType.SearchButtonbar:
                        break;
                    case ButtonBarType.TopDocumentButtonbar:
                        break;
                    case ButtonBarType.TranslationJobButtonbar:
                        break;
                    case ButtonBarType.TranslationJobContainerButtonbar:
                        break;
                    case ButtonBarType.TranslationMgmtReportButtonBar:
                        break;
                    case ButtonBarType.UserButtonbar:
                        break;
                    case ButtonBarType.UserGroupButtonbar:
                        break;
                    case ButtonBarType.UserRoleButtonbar:
                        break;
                    case ButtonBarType.XmlSettingsButtonBar:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
