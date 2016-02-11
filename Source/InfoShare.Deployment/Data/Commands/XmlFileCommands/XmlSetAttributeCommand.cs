using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands.XmlFileCommands
{
    public class XmlSetAttributeCommand : ICommand
    {
        private readonly string _xpath;
        private readonly string _value;
        private readonly IXmlConfigManager _xmlConfigManager;

        public XmlSetAttributeCommand(ILogger logger, string filePath, string xpath, string value)
        {
            _xpath = xpath;
            _value = value;
            _xmlConfigManager = new XmlConfigManager(logger, filePath);
        }
        
        public void Execute()
        {
            _xmlConfigManager.UncommentNode(_xpath);
        }
    }
}
