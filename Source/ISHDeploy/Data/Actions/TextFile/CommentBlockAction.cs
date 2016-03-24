using System.Collections.Generic;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.TextFile
{
    /// <summary>
    /// The action is responsible for commenting the block of text inside the text file.
    /// </summary>
    /// <seealso cref="SingleFileAction" />
    public class CommentBlockAction : SingleFileAction
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
        /// Initializes new instance of the <see cref="CommentBlockAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="searchPattern">Searched comment pattern</param>
        public CommentBlockAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }

        /// <summary>
        /// Initializes new instance of the <see cref="CommentBlockAction"/>
        /// </summary>
        /// <param name="logger">Instance of the <see cref="ILogger"/></param>
        /// <param name="filePath">Path to file that will be modified</param>
        /// <param name="searchPatterns">Searched comment patterns</param>
        public CommentBlockAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
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
                _textConfigManager.CommentBlock(FilePath, pattern);
            }
        }
    }
}
