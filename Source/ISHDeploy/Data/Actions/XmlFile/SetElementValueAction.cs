using ISHDeploy.Interfaces;
using ISHDeploy.Models;

namespace ISHDeploy.Data.Actions.XmlFile
{
    /// <summary>
    /// Action that sets new value for specified element.
    /// </summary>
    /// <seealso cref="SingleXmlFileAction" />
    public class SetElementValueAction : SingleXmlFileAction
    {
        /// <summary>
        /// The xpath to the searched element.
        /// </summary>
        private readonly string _xpath;

        /// <summary>
        /// The new value of element.
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetElementValueAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="xpath">The xpath to the element.</param>
        /// <param name="value">The new value of element.</param>
        public SetElementValueAction(ILogger logger, ISHFilePath filePath, string xpath, string value)
            : base(logger, filePath)
        {
            _xpath = xpath;
            _value = value;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            XmlConfigManager.SetElementValue(FilePath, _xpath, _value);
        }
    }
}
