using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;
using NSubstitute;

namespace InfoShare.Deployment.Tests
{
    public abstract class BaseUnitTest
    {
        protected readonly ILogger Logger = Substitute.For<ILogger>();
        protected readonly IFileManager FileManager = Substitute.For<IFileManager>();

        protected BaseUnitTest()
        {
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
