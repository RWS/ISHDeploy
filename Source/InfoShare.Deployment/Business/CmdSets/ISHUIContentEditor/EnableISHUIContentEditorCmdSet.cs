using System.Collections.Generic;
using System.IO;
using InfoShare.Deployment.Business.Invokers;
using InfoShare.Deployment.Data;
using InfoShare.Deployment.Data.Commands.XmlFileCommands;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor
{
    public class EnableISHUIContentEditorCmdSet : ICmdSet
    {
        private readonly CommandInvoker _invoker;

        public EnableISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation", enableBackup);

            // HINT: The sequence of commands depends on the product version

            var uncommentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut,
            };

            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.FolderButtonbar), uncommentPatterns));
            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.InboxButtonBar), uncommentPatterns));
            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LanguageDocumentButtonBar), uncommentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
