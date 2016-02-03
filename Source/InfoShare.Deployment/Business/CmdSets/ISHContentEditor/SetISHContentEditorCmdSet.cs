using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.FileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHContentEditor
{
    public class SetISHContentEditorCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public SetISHContentEditorCmdSet(ILogger logger, ISHProject ishProject, string licensePath, bool force)
        {
			_invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation");
			_invoker.AddCommand(new FileCopyCommand(logger, licensePath, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LicenceFolderPath), force));
		}

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
