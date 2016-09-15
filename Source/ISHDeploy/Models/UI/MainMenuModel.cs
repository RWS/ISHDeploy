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
using System.Xml.Serialization;

namespace ISHDeploy.Models.UI
{
    [XmlRoot("menuitem", Namespace = "")]
    public class MainMenuModel : BaseUIModel
    {
        [XmlAttribute("label")]
        public string Label { set; get; }

        [XmlElement("userrole")]
        public string[] UserRoles { set; get; }

        [XmlAttribute("action")]
        public string Action { set; get; }

        [XmlAttribute("id")]
        public string Id { set; get; }

        private MainMenuModel()
        {

        }

        public MainMenuModel(string label, string[] userRoles = null, string action = null, string id = null)
        {
            RelativeFilePath = @"Author\ASP\XSL\MainMenuBar.xml";
            RootPath = "mainmenubar";
            ChildItemPath = "menuitem";
            KeyAttribute = "label";

            Label = label;
            UserRoles = userRoles;
            Action = action;
            Id = id;
        }
    }
}
