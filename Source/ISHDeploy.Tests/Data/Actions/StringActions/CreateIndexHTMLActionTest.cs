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
using ISHDeploy.Data.Actions.StringActions;
using ISHDeploy.Data.Actions.WebAdministration;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;

namespace ISHDeploy.Tests.Data.Actions.WebAdministration
{
    [TestClass]
    public class CreateIndexHTMLActionTest : BaseUnitTest
    {
        private const string URL1 = "a href = \"https://ualaptop3.global.sdl.corp:443/InfoShareSTS/issue/wsfed?wa=wsignin1.0&wtrealm=https%3A%2F%2Fualaptop3.global.sdl.corp%2FInfoShareAuthor%2F\"";
        private const string URL2 = "a href = \"https://ualaptop3.global.sdl.corp:443/InfoShareSTS/issue/wsfed?wa=wsignin1.0&wtrealm=https%3A%2F%2Ftest.com%2FContentDelivery\"";
        private const string URL3 = "https://test.com/ContentDelivery";

        [TestMethod]
        [TestCategory("Actions")]
        public void CreateHTMLPageContent()
        {
            // Result
            string output = null;

            // Act
            (new CreateIndexHTMLAction(Logger,
                "https://UALAPTOP3.global.sdl.corp/InfoShareAuthor/",
                "https://UALAPTOP3.global.sdl.corp/InfoShareWS/Internal",
                "https://UALAPTOP3.global.sdl.corp/InfoShareSTS/",
                "test.com",
                "ContentDelivery",
               result => output = result)).Execute();

            // Assert
            Assert.IsTrue(output.Contains(URL1));
            Assert.IsTrue(output.Contains(URL2));
            Assert.IsTrue(output.Contains(URL3));        }
    }
}
