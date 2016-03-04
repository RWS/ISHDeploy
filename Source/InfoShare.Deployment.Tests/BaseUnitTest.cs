using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace InfoShare.Deployment.Tests
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
			var installParams =  new Dictionary<string, string>
			{
				{ "apppath", "."},
				{ "webpath", "."},
				{ "datapath", "."},
				{ "projectsuffix", ""}
			};

			return new ISHFilePath(new ISHDeployment(installParams, new Version("1.0.0.0")), ISHPaths.IshDeploymentType.Web, relativePath);
		}
    }
}
