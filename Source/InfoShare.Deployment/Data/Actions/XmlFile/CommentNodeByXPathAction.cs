using System.Collections.Generic;
using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
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
        /// Initializes a new instance of the <see cref="CommentNodeByXPathAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The xml file path.</param>
        /// <param name="xpaths">The xpaths to the nodes that needs to be commented.</param>
        public CommentNodeByXPathAction(ILogger logger, ISHFilePath filePath, IEnumerable<string> xpaths)
			: base(logger, filePath)
        {
            _xpaths = xpaths;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentNodeByXPathAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">Single xpath to the node that needs to be commented.</param>
        public CommentNodeByXPathAction(ILogger logger, ISHFilePath filePath, string xpath)
            : this(logger, filePath, new[] { xpath })
        { }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            foreach (var xpath in _xpaths)
            {
                XmlConfigManager.CommentNode(FilePath, xpath);
            }
        }
    }
}
