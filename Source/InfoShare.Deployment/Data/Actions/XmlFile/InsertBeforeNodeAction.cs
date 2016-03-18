using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that removes single node in xml file found by xpaths.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class InsertBeforeNodeAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xpath to the xml node.
        /// </summary>
        private readonly string _xpath;
		
		/// <summary>
		/// The xpath to the xml node.
		/// </summary>
		private readonly string _xpathBeforeNode;

		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveSingleNodeAction"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="filePath">The xml file path.</param>
		/// <param name="xpath">The xpath to the node that needs to be removed.</param>
		/// <param name="xpathBeforeNode">The xpath to the node that needs to be removed.</param>
		public InsertBeforeNodeAction(ILogger logger, ISHFilePath filePath, string xpath, string xpathBeforeNode = null)
			: base(logger, filePath)
        {
            _xpath = xpath;
			_xpathBeforeNode = xpathBeforeNode;
		}

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
			XmlConfigManager.InsertBeforeNode(FilePath, _xpath, _xpathBeforeNode);
        }
    }
}
