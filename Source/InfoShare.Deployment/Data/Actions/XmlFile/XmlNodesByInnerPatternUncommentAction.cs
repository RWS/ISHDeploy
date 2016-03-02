using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlNodesByInnerPatternUncommentAction : SingleXmlFileAction
    {
        private readonly string _searchPattern;

        public XmlNodesByInnerPatternUncommentAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : base(logger, filePath)
        {
            _searchPattern = searchPattern;
        }

        /// <summary>
        /// Performs the action
        /// </summary>
        public override void Execute()
        {
            XmlConfigManager.UncommentNodesByInnerPattern(FilePath, _searchPattern);
        }
    }
}
