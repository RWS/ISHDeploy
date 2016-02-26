using System.Collections.Generic;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlNodeCommentAction : SingleXmlFileAction
    {
        private readonly IEnumerable<string> _xpaths;

        public XmlNodeCommentAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> xpaths)
			: base(logger, filePath)
		{
            _xpaths = xpaths;
        }

        public XmlNodeCommentAction(ILogger logger, ISHFilePath filePath, string xpath)
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
