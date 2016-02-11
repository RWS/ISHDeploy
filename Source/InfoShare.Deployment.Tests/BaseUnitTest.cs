using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using NSubstitute;

namespace InfoShare.Deployment.Tests
{
    public abstract class BaseUnitTest
    {
        public ILogger Logger = Substitute.For<ILogger>();
        public IFileManager FileManager = Substitute.For<IFileManager>();

        protected BaseUnitTest()
        {
            ObjectFactory.SetInstance(FileManager);
        }


        public XElement GetXElementByXPath(XDocument doc, string xpath)
        {
            return doc.XPathSelectElement(xpath);
        }
    }
}
