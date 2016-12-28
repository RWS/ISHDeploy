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
﻿using System;
﻿using ISHDeploy.Common;
﻿using ISHDeploy.Data.Actions.XmlFile;
using ISHDeploy.Data.Managers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace ISHDeploy.Tests.Data.Actions
{
    [TestClass]
    public class SingleFileActionTest : BaseUnitTest
    {
		[TestInitialize]
		public void TestInitializer()
		{
			ObjectFactory.SetInstance(Substitute.For<IXmlConfigManager>());
		}

		[TestMethod]
        [TestCategory("Actions")]
        public void Create_Backup_file_created()
        {
			// Arrange
			var testFilePath = GetIshFilePath("Test.xml");
	        var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".bak");

			FileManager.FileExists(testFilePath.AbsolutePath).Returns(true);
			FileManager.FileExists(testFilePath.VanillaPath).Returns(true);

			// Act
			new SetAttributeValueAction(Logger, testFilePath, "", "", "");

			// Assert
			FileManager.Received(1).Copy(testFilePath.AbsolutePath, backUpFilePath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Dispose_Backup_file_disposed()
		{
			// Arrange
			var testFilePath = GetIshFilePath("Test.xml");
			var backUpFilePath = String.Concat(testFilePath.AbsolutePath, ".bak");

			FileManager.FileExists(testFilePath.AbsolutePath).Returns(true);
			FileManager.FileExists(backUpFilePath).Returns(true);
			FileManager.FileExists(testFilePath.VanillaPath).Returns(true);

			// Act
			(new SetAttributeValueAction(Logger, testFilePath, "", "", "")).Dispose();

			// Assert
			FileManager.Received(1).Delete(backUpFilePath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}

		[TestMethod]
		[TestCategory("Actions")]
		public void Create_Vanilla_backup_created()
		{
			// Arrange
			var testFilePath = GetIshFilePath("Test.xml");
			FileManager.FileExists(testFilePath.AbsolutePath).Returns(true);

			// Act
			new SetAttributeValueAction(Logger, testFilePath, "", "", "");

			// Assert
			FileManager.Received(1).Copy(testFilePath.AbsolutePath, testFilePath.VanillaPath);
			Logger.DidNotReceive().WriteWarning(Arg.Any<string>());
		}
	}
}
