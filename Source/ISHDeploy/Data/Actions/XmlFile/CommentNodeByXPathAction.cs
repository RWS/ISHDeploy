using System.Collections.Generic;
using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that comments nodes in xml file found by xpaths.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class CommentNodeByXPathAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xpath to the xml node.
        /// </summary>
        private readonly IEnumerable<string> _xpaths;

        /// <summary>
        /// The identifier to encode inner XML.
        /// </summary>
        private readonly bool _encodeInnerXml;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentNodeByXPathAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The xml file path.</param>
        /// <param name="xpaths">The xpaths to the nodes that needs to be commented.</param>
        /// <param name="encodeInnerXml">True if content of the comment should be encoded; otherwise False.</param>
        public CommentNodeByXPathAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> xpaths, bool encodeInnerXml = false)
			: base(logger, filePath)
        {
            _xpaths = xpaths;
            _encodeInnerXml = encodeInnerXml;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentNodeByXPathAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">Single xpath to the node that needs to be commented.</param>
        /// <param name="encodeInnerXml">True if content of the comment should be encoded; otherwise False.</param>
        public CommentNodeByXPathAction(ILogger logger, ISHFilePath filePath, string xpath, bool encodeInnerXml = false)
            : this(logger, filePath, new[] { xpath }, encodeInnerXml)
        { }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            foreach (var xpath in _xpaths)
            {
                XmlConfigManager.CommentNode(FilePath, xpath, _encodeInnerXml);
            }
        }
    }
}
