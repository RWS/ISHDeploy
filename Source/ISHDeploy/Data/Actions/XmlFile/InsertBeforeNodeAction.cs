using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that inserts new node before specified one.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class InsertBeforeNodeAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xpath to the searched node.
        /// </summary>
        private readonly string _xpath;

        /// <summary>
        /// The new node as a XML string.
        /// </summary>
        private readonly string _xmlString;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertBeforeNodeAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node before which we want to add a new node.</param>
        /// <param name="xmlString">The new node as a XML string.</param>
        public InsertBeforeNodeAction(ILogger logger, ISHFilePath filePath, string xpath, string xmlString)
            : base(logger, filePath)
        {
            _xpath = xpath;
            _xmlString = xmlString;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            XmlConfigManager.InsertBeforeNode(FilePath, _xpath, _xmlString);
        }
    }
}
