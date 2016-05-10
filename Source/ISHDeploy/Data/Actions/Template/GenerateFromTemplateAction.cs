using System.Collections.Generic;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Template
{
    /// <summary>
    /// Generates content from resource template.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class GenerateFromTemplateAction : SingleFileCreationAction
    {
        /// <summary>
        /// The template file path.
        /// </summary>
        private readonly string _templateFilePath;

        /// <summary>
        /// The output file path.
        /// </summary>
        private readonly string _outputFilePath;

        /// <summary>
        /// Key-Value pair of input parameters to be found and replaced in template
        /// </summary>
        private readonly Dictionary<string, string> _inputParameters;

        /// <summary>
        /// The template manager
        /// </summary>
        private readonly ITemplateManager _templateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateFromTemplateAction" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="templateFilePath">The template file path.</param>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="inputParameters">The input parameters.</param>
        public GenerateFromTemplateAction(ILogger logger,
            string templateFilePath,
            string outputFilePath,
            Dictionary<string, string> inputParameters) : base(logger, outputFilePath)
        {
            _templateManager = ObjectFactory.GetInstance<ITemplateManager>();

            _templateFilePath = templateFilePath;
            _outputFilePath = outputFilePath;
            _inputParameters = inputParameters;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var outputContent = _templateManager.GenerateDocument(_templateFilePath, _inputParameters);

            _fileManager.Write(_outputFilePath, outputContent);
        }
    }
}
