using System;
using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.LicenseCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHContentEditor
{
    public class TestISHContentEditorCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public TestISHContentEditorCmdSet(ILogger logger, ISHProject ishProject, string hostname, Action<bool> isValid)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation");
            _invoker.AddCommand(new LicenseTestCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LicenceFolderPath), hostname, isValid));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
