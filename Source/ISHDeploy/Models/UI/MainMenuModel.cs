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
using System.Xml.Linq;

namespace ISHDeploy.Models.UI
{
    class MainMenuModel : UIBase
    {
        [UIBindingAttribute ("label")]
        public string _label { set; get; }
        [UIBindingAttribute("userrole", true)]
        public string[] _userRole { set; get; }
        [UIBindingAttribute("action")]
        public string _action { set; get; }
        [UIBindingAttribute("id")]
        public string _id { set; get; }
        public MainMenuModel(string label, string[] userRole, string action, string id)
        {
            filePath = @"Author\ASP\XSL\MainMenuBar.xml";
            rootPath = "mainmenubar";
            childItemPath = "menuitem";
            keyAttribute = "label";
            element = new XElement(childItemPath);

            _label = label;
            _userRole = userRole;
            _action = action;
            _id = id;
        }
    }
}
