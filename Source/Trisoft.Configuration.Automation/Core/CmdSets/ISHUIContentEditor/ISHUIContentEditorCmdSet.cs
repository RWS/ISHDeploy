using System;
using Trisoft.Configuration.Automation.Core.Commands;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.CmdSets.ISHUIContentEditor
{
    public class ISHUIContentEditorCmdSet : IExecutable
    {
        private readonly ILogger _logger;
        private readonly CommandInvoker _invoker;
        private readonly bool _isBackupEnabled;

        public ISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _logger = logger;
            _isBackupEnabled = enableBackup;
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation");

            // The sequence of commands depends on the product version
            //if (ishProject.Version.CompareTo(specipicVersion))
            //{
            //  _invoker.AddCommand(specipicCommand);
            //}

            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\FolderButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\InboxButtonBar.xml", "XOPUS ADD \"UNDO CHECK OUT\""));
            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\LanguageDocumentButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
        }

        public void Execute()
        {
            try
            {
                if (_isBackupEnabled)
                {
                    _invoker.Backup();
                }
                _invoker.Invoke();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex);
                if (_isBackupEnabled)
                {
                    _invoker.Rollback();
                }
            }
        }
    }
}
