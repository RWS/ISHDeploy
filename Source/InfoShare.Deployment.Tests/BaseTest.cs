using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;
using NSubstitute;

namespace InfoShare.Deployment.Tests
{
    public abstract class  BaseTest
    {
        protected readonly ISHProject IshProject;
        public ILogger Logger = Substitute.For<ILogger>();

        public const string XPathCheckOutWithXopusButton = "BUTTONBAR/BUTTON/INPUT[@NAME='CheckOutWithXopus']";
        public const string XPathUndoCheckOutButton = "BUTTONBAR/BUTTON/INPUT[@NAME='undoCheckOut']";
        public const string XPathCheckOutButton = "BUTTONBAR/BUTTON/INPUT[@NAME='checkOut']";
        public const string XPathCheckInButton = "BUTTONBAR/BUTTON/INPUT[@NAME='checkIn']";

        protected BaseTest()
        {
            ObjectFactory.SetInstance<IFileManager, FileManager>(new FileManager(Logger));
            IshProject = new ISHProject(new Dictionary<string,string> { { "webpath", @".\TestData" } }, new Version());
        }

        public string GetPathToFile(string relativeFilePath)
        {
            return Path.Combine(IshProject.AuthorFolderPath, relativeFilePath);
        }

        public XElement GetXElementByXPath(string filePath, string xpath)
        {
            var doc = XDocument.Load(filePath);
            return doc.XPathSelectElement(xpath);
        }
    }
}
