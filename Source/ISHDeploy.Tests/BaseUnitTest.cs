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

using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using ISHDeploy.Common;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Common.Interfaces;
using ISHDeploy.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests
{
    [TestClass] 
    public abstract class BaseUnitTest
    {
        protected ILogger Logger;
        protected IFileManager FileManager;

        [TestInitialize]
        public void BaseTestInitialize()
        {
            Logger = Substitute.For<ILogger>();
            FileManager = Substitute.For<IFileManager>();

            ObjectFactory.SetInstance(FileManager);
        }

        public XElement GetXElementByXPath(XDocument doc, string xpath)
        {
            return doc.XPathSelectElement(xpath);
        }

	    public ISHFilePath GetIshFilePath(string relativePath)
	    {
			var installParams = new Dictionary<string, string>
            {
                ["projectsuffix"] = string.Empty,
                ["apppath"] = string.Empty,
                ["webpath"] = string.Empty,
                ["datapath"] = string.Empty,
                ["databasetype"] = string.Empty,
                ["baseurl"] = "https://",
                ["infoshareauthorwebappname"] = string.Empty,
                ["infosharewswebappname"] = string.Empty,
                ["infosharestswebappname"] = string.Empty,
                ["websitename"] = string.Empty
            };

            return new ISHFilePath("Web", "Backup", relativePath);
        }
    }
}
