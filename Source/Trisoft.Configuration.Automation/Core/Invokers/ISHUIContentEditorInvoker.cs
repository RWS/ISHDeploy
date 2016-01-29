using Trisoft.Configuration.Automation.Core.Commands;
using Trisoft.Configuration.Automation.Core.Model;

namespace Trisoft.Configuration.Automation.Core.Invokers
{
    public class ISHUIContentEditorInvoker : CommandInvoker<ICommand>
    {
        public ISHUIContentEditorInvoker(ILogger logger, ISHProject ishProject, bool enableBackup)
            : base(logger, ishProject, enableBackup, "InfoShare ContentEditor activation")
        {
            this.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\FolderButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
            this.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\InboxButtonBar.xml", "XOPUS ADD \"UNDO CHECK OUT\""));
            this.AddCommand(new XmlUncommentCommand(logger, $"{ishProject.AuthorFolderPath}\\ASP\\XSL\\LanguageDocumentButtonbar.xml", "XOPUS ADD \"CHECK OUT WITH XOPUS\""));
        }
    }
}
