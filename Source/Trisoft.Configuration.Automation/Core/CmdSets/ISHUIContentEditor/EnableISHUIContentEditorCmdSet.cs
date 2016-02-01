using System;
using System.Collections.Generic;
using System.IO;
using Trisoft.Configuration.Automation.Core.Commands;
using Trisoft.Configuration.Automation.Core.Invokers;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.CmdSets.ISHUIContentEditor
{
    public class EnableISHUIContentEditorCmdSet : ICmdSet
    {
        private readonly ILogger _logger;
        private readonly CommandInvoker _invoker;

        public EnableISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _logger = logger;
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation", enableBackup);

            // HINT: The sequence of commands depends on the product version
            //if (ishProject.Version.CompareTo(specipicVersion))
            //{
            //  _invoker.AddCommand(specipicCommand);
            //}

            var uncommentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut,
            };

            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.FolderButtonbar), uncommentPatterns));

            //TODO: ask what to do with that? uncommentPatterns.Add(CommentPatterns.XopusRemoveCheckoutDownload);
            //TODO: ask what to do with that? uncommentPatterns.Add(CommentPatterns.XopusRemoveCheckIn);
            //TODO: ask what to do with that? uncommentPatterns.Add(CommentPatterns.XopusRemoveUndoCheckOut);
            
            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.InboxButtonBar), uncommentPatterns));
            _invoker.AddCommand(new XmlUncommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LanguageDocumentButtonBar), uncommentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
