using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
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
    }
}
