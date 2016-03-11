using InfoShare.Deployment.Data.Managers.Interfaces;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that comments multiple nodes with searched pattern that can be located inside or preced the searched nodes.
    /// </summary>
    /// <example> Example of searched node "BUTTON" that contains "Translation" comment inside <![CDATA[
    /// <BUTTONBAR>
    ///    <BUTTON>
    ///         <!-- TRANSLATION -->
    ///         <INPUT type = "button" />
    ///     </ BUTTON >
    /// </ BUTTONBAR >
    /// ]]></example>
    /// <example> Example of searched node with "Translation" comment precedes the searched node "BUTTON" <![CDATA[
    /// <BUTTONBAR>
    ///    <BUTTON>
    ///         <!-- TRANSLATION -->
    ///         <INPUT type = "button" />
    ///     </ BUTTON >
    /// </ BUTTONBAR >
    /// ]]></example>
    /// <seealso cref="SingleFileAction" />
    public class CommentNodesByInnerPatternAction : SingleFileAction
    {
        /// <summary>
        /// The xpath to the searched node.
        /// </summary>
        private readonly string _xpath;

        /// <summary>
        /// The pattern that that acts like placeholder.
        /// </summary>
        private readonly string _patternElem;

        /// <summary>
        /// The xml configuration manager
        /// </summary>
        private readonly IXmlConfigManager _xmlConfigManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentNodesByInnerPatternAction"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> instance.</param>
        /// <param name="filePath">Path to file that will be modified.</param>
        /// <param name="xpath">XPath to node that contains <paramref name="patternElem"/></param>
        /// <param name="patternElem">Comment pattern, that is used for searching node.</param>
        public CommentNodesByInnerPatternAction(ILogger logger, ISHFilePath filePath, string xpath, string patternElem)
            : base(logger, filePath)
        {
            _xpath = xpath;
            _patternElem = patternElem;

            _xmlConfigManager = ObjectFactory.GetInstance<IXmlConfigManager>();
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            // Try to get rid of inner patterns in the file, if there are some. Will be done only once.
            _xmlConfigManager.MoveOutsideInnerComment(FilePath, _xpath, _patternElem);

            // Comment nodes in usual way
            _xmlConfigManager.CommentNodesByPrecedingPattern(FilePath, _patternElem);
        }
    }
}
