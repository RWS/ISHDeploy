using Trisoft.Configuration.Automation.Core.Commands;
using Trisoft.Configuration.Automation.Core.Invokers;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.CmdSets.ISHUIContentEditor
{
    public class ISHUIContentEditorCmdSet
    {
        private readonly ICommandInvoker<ICommand> _invoker; 

        public ISHUIContentEditorCmdSet(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _invoker = new CommandInvoker<ICommand>(logger, ishProject, enableBackup, "InfoShare ContentEditor activation");

            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\FolderButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\InboxButtonBar.xml", "XOPUS ADD \"UNDO CHECK OUT\""));
            _invoker.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\LanguageDocumentButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
