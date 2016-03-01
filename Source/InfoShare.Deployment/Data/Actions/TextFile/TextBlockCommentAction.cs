using System.Collections.Generic;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions.TextFile
{
    public class TextBlockCommentAction : BaseAction
    {
        private readonly IEnumerable<string> _searchPatterns;
        private readonly ITextConfigManager _textConfigManager;
        private readonly string _filePath;

        public TextBlockCommentAction(ILogger logger, string filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }

        public TextBlockCommentAction(ILogger logger, string filePath, IEnumerable<string> searchPatterns)
            : base(logger)
        {
            _filePath = filePath;

            _searchPatterns = searchPatterns;
            _textConfigManager = ObjectFactory.GetInstance<ITextConfigManager>();
        }

        public override void Execute()
        {
            foreach (var pattern in _searchPatterns)
            {
                _textConfigManager.CommentBlock(_filePath, pattern);
            }
        }
    }
}
