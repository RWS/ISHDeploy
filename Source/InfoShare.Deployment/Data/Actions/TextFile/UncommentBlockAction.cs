using System.Collections.Generic;
using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.TextFile
{
    /// <summary>
    /// The action the is responsible for uncommenting block of text inside the text file.
    /// </summary>
    public class UncommentBlockAction : SingleFileAction
    {
        /// <summary>
        /// The searched placeholders.
        /// </summary>
        private readonly IEnumerable<string> _searchPatterns;

        /// <summary>
        /// The text configuration manager.
        /// </summary>
        private readonly ITextConfigManager _textConfigManager;

        /// <summary>
        /// Initializes new instance of the <see cref="UncommentBlockAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="searchPattern">Searched comment pattern</param>
        public UncommentBlockAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="UncommentBlockAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="searchPatterns">Searched comment patterns</param>
        public UncommentBlockAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
            : base(logger, filePath)
        {
            _searchPatterns = searchPatterns;
            _textConfigManager = ObjectFactory.GetInstance<ITextConfigManager>();
        }

        /// <summary>
        /// Executes current action.
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
