using System.Collections.Generic;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that comments nodes in the xml that follow after comment with specific placeholders.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class CommentNodesByPrecedingPatternAction : SingleXmlFileAction
    {
        /// <summary>
        /// The search placeholders.
        /// </summary>
        private readonly IEnumerable<string> _searchPatterns;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentNodesByPrecedingPatternAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="searchPatterns">The search placeholders.</param>
        public CommentNodesByPrecedingPatternAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns)
			: base(logger, filePath)
        {
            _searchPatterns = searchPatterns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentNodesByPrecedingPatternAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="searchPattern">Single search placeholder.</param>
        public CommentNodesByPrecedingPatternAction(ILogger logger, ISHFilePath filePath, string searchPattern)
            : this(logger, filePath, new[] { searchPattern })
        { }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            foreach (var pattern in _searchPatterns)
            {
                XmlConfigManager.CommentNodesByPrecedingPattern(FilePath, pattern);
            }
        }
    }
}
