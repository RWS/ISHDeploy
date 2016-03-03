using System.Collections.Generic;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class CommentNodeByXPathAction : SingleXmlFileAction
    {
        private readonly IEnumerable<string> _xpaths;

        public CommentNodeByXPathAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> xpaths)
			: base(logger, filePath)
        {
            _xpaths = xpaths;
        }

        public CommentNodeByXPathAction(ILogger logger, ISHFilePath filePath, string xpath)
            : this(logger, filePath, new[] { xpath })
        { }

        public override void Execute()
        {
            foreach (var xpath in _xpaths)
            {
                XmlConfigManager.CommentNode(FilePath, xpath);
            }
        }
    }
}
