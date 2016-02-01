using System.IO;
using InfoShare.Deployment.Interfaces;
using NSubstitute;

namespace InfoShare.Deployment.Tests.Data.Commands
{
    public class BaseCommandTest
    {
        public ILogger Logger = Substitute.For<ILogger>();

        public const string XPathCheckOutWithXopusButton = "BUTTONBAR/BUTTON/INPUT[@NAME='CheckOutWithXopus']";
        public const string XPathUndoCheckOutButton = "BUTTONBAR/BUTTON/INPUT[@NAME='undoCheckOut']";
        private const string ProjectRelativePath = @".\TestData";

        public string GetPathToFile(string relativeFilePath)
        {
            return Path.Combine(ProjectRelativePath, relativeFilePath);
        }
    }
}
