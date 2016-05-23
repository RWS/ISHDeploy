using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that removes single node in xml file found by xpaths.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class RemoveSingleNodeAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xpath to the xml node.
        /// </summary>
        private readonly string _xpath;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveSingleNodeAction"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="filePath">The xml file path.</param>
		/// <param name="xpath">The xpath to the node that needs to be removed.</param>
		public RemoveSingleNodeAction(ILogger logger, ISHFilePath filePath, string xpath)
			: base(logger, filePath)
        {
            _xpath = xpath;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
			XmlConfigManager.RemoveSingleNode(FilePath, _xpath);
        }
    }
}
