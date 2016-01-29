using Trisoft.Configuration.Automation.Core.Managers;

namespace Trisoft.Configuration.Automation.Core.Commands
{
    public class XmlUncommentCommand : BaseCommand, IRestorable
    {
        private readonly string _commentedLineContains;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlUncommentCommand(ILogger logger, string filePath, string commentedLineContains)
            : base(logger)
        {
            _xmlConfigManager = new XmlConfigManager(logger, filePath);
            _commentedLineContains = commentedLineContains;
        }

        public void Backup()
        {
            _xmlConfigManager.Backup();
        }

        public void Execute()
        {
            //if (_xmlConfigManager.IsNodeCommented(_commentedLineContains))
            //{
            //    _xmlConfigManager.UncommentNode(_commentedLineContains);
            //}
            _xmlConfigManager.UncommentNode(_commentedLineContains);
        }

        public void Rollback()
        {
            _xmlConfigManager.RestoreOriginal();
        }
    }
}
