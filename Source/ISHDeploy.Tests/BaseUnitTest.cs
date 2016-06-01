using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;
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
                ["infosharestswebappname"] = string.Empty
            };

			return new ISHFilePath(new ISHDeploymentExtended(installParams, new Version("1.0.0.0")), ISHFilePath.IshDeploymentType.Web, relativePath);
		}
    }
}
