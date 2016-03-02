using System.Collections.Generic;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.TextFile
{
    /// <summary>
    /// Uncomments block of text inside text file
    /// </summary>
    public class TextBlockUncommentAction : SingleFileAction
    {
        private readonly IEnumerable<string> _searchPatterns;
        private readonly ITextConfigManager _textConfigManager;

        /// <summary>
        /// Initialized new instance of the <see cref="TextBlockUncommentAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="searchPattern">Searched comment pattern</param>
        public TextBlockUncommentAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }

        /// <summary>
        /// Initialized new instance of the <see cref="TextBlockUncommentAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="searchPatterns">Searched comment patterns</param>
        public TextBlockUncommentAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
            : base(logger, filePath)
        {
            _searchPatterns = searchPatterns;
            _textConfigManager = ObjectFactory.GetInstance<ITextConfigManager>();
        }

        /// <summary>
        /// Performs action
        /// </summary>
        public override void Execute()
        {
            foreach (var pattern in _searchPatterns)
            {
                _textConfigManager.UncommentBlock(FilePath, pattern);
            }
        }
    }
}
