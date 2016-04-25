using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that sets specific node attribute to the certain value.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class SetAttributeValueAction : SingleXmlFileAction
    {
        /// <summary>
        /// The attribute xPath.
        /// </summary>
        private readonly string _attributeXpath;

        /// <summary>
        /// The attribute value.
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetAttributeValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The attribute new value.</param>
        public SetAttributeValueAction(ILogger logger, ISHFilePath filePath, string xpath, string attributeName, string value)
            : this(logger, filePath, string.Concat(xpath, "\\@", attributeName), value)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="SetAttributeValueAction"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="filePath">The file path.</param>
		/// <param name="attributeXpath">The xpath to the node.</param>
		/// <param name="value">The attribute new value.</param>
		public SetAttributeValueAction(ILogger logger, ISHFilePath filePath, string attributeXpath, string value)
			: base(logger, filePath)
		{
			_attributeXpath = attributeXpath;
			_value = value;
		}

		/// <summary>
		/// Executes current action.
		/// </summary>
		public override void Execute()
        {
            XmlConfigManager.SetAttributeValue(FilePath, _attributeXpath, _value);
        }
    }
}
