using System.Collections.Generic;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlNodesByInnerPatternUncommentAction : SingleXmlFileAction
    {
        private readonly IEnumerable<string> _searchPatterns;

        public XmlNodesByInnerPatternUncommentAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new [] { searchPattern })
        { }

        public XmlNodesByInnerPatternUncommentAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
            : base(logger, filePath)
        {
            _searchPatterns = searchPatterns;
        }

        /// <summary>
        /// Performs the action
        /// </summary>
        public override void Execute()
        {
            foreach (var searchPattern in _searchPatterns)
            {
                XmlConfigManager.UncommentNodesByInnerPattern(FilePath, searchPattern);
            }
        }
    }
}
