using System.Collections.Generic;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that uncomments nodes in the xml that contain comment node with specific placeholders inside.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class UncommentNodesByInnerPatternAction : SingleXmlFileAction
    {
        /// <summary>
        /// The search placeholders.
        /// </summary>
        private readonly IEnumerable<string> _searchPatterns;

        /// <summary>
        /// The identifier to decode inner XML.
        /// </summary>
        private readonly bool _decodeInnerXml;

        /// <summary>
        /// Initializes a new instance of the <see cref="UncommentNodesByInnerPatternAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="searchPatterns">The search placeholders.</param>
        /// <param name="decodeInnerXml">True if content of the comment should be decoded; otherwise False.</param>
        public UncommentNodesByInnerPatternAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> searchPatterns, bool decodeInnerXml = false)
            : base(logger, filePath)
        {
            _searchPatterns = searchPatterns;
            _decodeInnerXml = decodeInnerXml;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UncommentNodesByInnerPatternAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="searchPattern">Single search placeholder.</param>
        /// <param name="decodeInnerXml">True if content of the comment should be decoded; otherwise False.</param>
        public UncommentNodesByInnerPatternAction(ILogger logger, ISHFilePath filePath, string searchPattern, bool decodeInnerXml = false)
            : this(logger, filePath, new [] { searchPattern }, decodeInnerXml)
        { }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            foreach (var searchPattern in _searchPatterns)
            {
                XmlConfigManager.UncommentNodesByInnerPattern(FilePath, searchPattern, _decodeInnerXml);
            }
        }
    }
}
