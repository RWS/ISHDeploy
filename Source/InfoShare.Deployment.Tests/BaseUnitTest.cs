using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
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
    }
}
