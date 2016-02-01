using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trisoft.Configuration.Automation.Core.Commands;
using Trisoft.Configuration.Automation.Core.Invokers;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.CmdSets.ISHUIContentEditor
{
    public class DisableISHUIContentEditorCmdSet : ICmdSet
    {
        private readonly ILogger _logger;
        private readonly CommandInvoker _invoker;

        public DisableISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _logger = logger;
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation", enableBackup);

            var commentPatterns = new List<string>
            {
                CommentPatterns.XopusAddCheckOut,
                CommentPatterns.XopusAddUndoCheckOut,
            };

            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.FolderButtonbar), commentPatterns));

            //TODO: ask what to do with that? commentPatterns.Add(CommentPatterns.XopusRemoveCheckoutDownload);
            //TODO: ask what to do with that? commentPatterns.Add(CommentPatterns.XopusRemoveCheckIn);
            //TODO: ask what to do with that? commentPatterns.Add(CommentPatterns.XopusRemoveUndoCheckOut);

            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.InboxButtonBar), commentPatterns));
            _invoker.AddCommand(new XmlCommentCommand(logger, Path.Combine(ishProject.AuthorFolderPath, ISHPaths.LanguageDocumentButtonBar), commentPatterns));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
