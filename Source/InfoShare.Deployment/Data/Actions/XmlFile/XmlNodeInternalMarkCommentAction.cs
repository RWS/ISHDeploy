using System.Collections.Generic;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlNodeInternalMarkCommentAction : BaseAction
    {
        private readonly string _internalXPath;
        private readonly string _externalXPath;
        private readonly string _patternElem;
        private readonly IXmlConfigManager _xmlConfigManager;
        private readonly string _filePath;

        public XmlNodeInternalMarkCommentAction(ILogger logger, string filePath, string internalXPath, string externalXPath, string patternElem)
            : base(logger)
        {
            _filePath = filePath;
            _internalXPath = internalXPath;
            _externalXPath = externalXPath;
            _patternElem = patternElem;

            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }

        public override void Execute()
        {
            if (_xmlConfigManager.XPathExists(_filePath, _internalXPath))
            {
                _xmlConfigManager.CommentNodeWithInternalPattern(_filePath, _internalXPath, _patternElem);
                return;
            }

            _xmlConfigManager.CommentNode(_filePath, _externalXPath);
        }
    }
}
