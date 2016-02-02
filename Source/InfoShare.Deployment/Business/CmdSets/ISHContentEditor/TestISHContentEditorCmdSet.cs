using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.FileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHContentEditor
{
    public class TestISHContentEditorCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public TestISHContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation", enableBackup);

            _invoker.AddCommand(new FileTestCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LicenceFolderPath)));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
