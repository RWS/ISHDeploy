using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlSetAttributeValueCommand : ICommand
    {
        private readonly string _filePath;
        private readonly string _xpath;
        private readonly string _attributeName;
        private readonly string _value;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlSetAttributeValueCommand(ILogger logger, string filePath, string xpath, string attributeName, string value)
        {
            _filePath = filePath;
            _xpath = xpath;
            _attributeName = attributeName;
            _value = value;
            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }
        
        public void Execute()
        {
            _xmlConfigManager.SetAttributeValue(_filePath, _xpath, _attributeName, _value);
        }
    }
}
