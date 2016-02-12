using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlSetAttributeValueCommand : BaseCommand
    {
        private readonly string _filePath;
        private readonly string _xpath;
        private readonly string _attributeName;
        private readonly string _value;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlSetAttributeValueCommand(ILogger logger, string filePath, string xpath, string attributeName, string value)
            : base(logger)
        {
            _filePath = filePath;
            _xpath = xpath;
            _attributeName = attributeName;
            _value = value;
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }
        
        public override void Execute()
        {
            _xmlConfigManager.SetAttributeValue(_filePath, _xpath, _attributeName, _value);
        }
    }
}
