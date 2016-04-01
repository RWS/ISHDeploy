using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
	/// <summary>
	/// Action that sets specific node attribute to the certain value.
	/// </summary>
	/// <seealso cref="ISHDeploy.Data.Actions.SingleXmlFileAction" />
	/// <seealso cref="SingleXmlFileAction" />
	public class SetNodeAction : SingleXmlFileAction
	{
		/// <summary>
		/// The xpath to the searched node.
		/// </summary>
		private readonly string _xpath;

		/// <summary>
		/// ISH configuration XML Node.
		/// </summary>
		private readonly IISHXmlNode _ishXmlNode;

		/// <summary>
		/// Implements Set node action.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="xpath">The xpath to the node.</param>
		/// <param name="ishXmlNode">The ish XML node.</param>
		public SetNodeAction(ILogger logger, ISHFilePath filePath, string xpath, IISHXmlNode ishXmlNode)
            : base(logger, filePath)
        {
			_xpath = xpath;
			_ishXmlNode = ishXmlNode;
		}

		/// <summary>
		/// Executes current action.
		/// </summary>
		public override void Execute()
		{
			XmlConfigManager.SetNode(FilePath, _xpath, _ishXmlNode);
		}
	}
}
