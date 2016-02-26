using System.Collections.Generic;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    public class XmlBlockCommentAction : SingleXmlFileAction
	{
        private readonly IEnumerable<string> _searchPatterns;

        public XmlBlockCommentAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
			: base(logger, filePath)
		{
            _searchPatterns = searchPatterns;
        }

        public XmlBlockCommentAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }

        public override void Execute()
        {
            foreach (var pattern in _searchPatterns)
            {
                XmlConfigManager.CommentBlock(FilePath, pattern);
            }
        }
    }
}
