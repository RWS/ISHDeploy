using System;
using Trisoft.Configuration.Automation.Core.Commands;
using Trisoft.Configuration.Automation.Core.Invokers;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.CmdSets.ISHUIContentEditor
{
    public class ISHUIContentEditorCmdSet : ICmdSet
    {
        private readonly ILogger _logger;
        private readonly CommandInvoker _invoker;

        public ISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _logger = logger;
            _invoker = new CommandInvoker(logger, "InfoShare ContentEditor activation", enableBackup);

            // The sequence of commands depends on the product version
            //if (ishProject.Version.CompareTo(specipicVersion))
            //{
            //  _invoker.AddCommand(specipicCommand);
            //}

            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\FolderButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\InboxButtonBar.xml", "XOPUS ADD \"UNDO CHECK OUT\""));
            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\LanguageDocumentButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
        }

        public void Run()
        {
            try
            {
                _invoker.Invoke();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex);
            }
        }
    }
}
