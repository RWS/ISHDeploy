using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlSetAttributeValueAction : SingleXmlFileAction
	{
        private readonly string _xpath;
        private readonly string _attributeName;
        private readonly string _value;

        public XmlSetAttributeValueAction(ILogger logger, ISHFilePath filePath, string xpath, string attributeName, string value)
            : base(logger, filePath)
        {
            _xpath = xpath;
            _attributeName = attributeName;
            _value = value;
        }
        
        public override void Execute()
        {
            XmlConfigManager.SetAttributeValue(FilePath, _xpath, _attributeName, _value);
        }
    }
}
