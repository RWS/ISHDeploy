using System.Collections.Generic;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.File
{
    /// <summary>
    /// Saves document generated from resource template.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class FileGenerateFromTemplateAction : SingleFileCreationAction
    {
        /// <summary>
        /// The path to template file.
        /// </summary>
        private readonly string _templateFilePath;

        /// <summary>
        /// The output file path.
        /// </summary>
        private readonly string _outputFilePath;

        /// <summary>
        /// The set of parameters.
        /// </summary>
        private readonly Dictionary<string, string> _parameters;

        /// <summary>
        /// The template manager/
        /// </summary>
        private readonly ITemplateManager _templateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileGenerateFromTemplateAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="templateFilePath">The path to template file.</param>
        /// <param name="parameters">The set of parameters.</param>
        public FileGenerateFromTemplateAction(ILogger logger,
            string templateFilePath,
            string outputFilePath,
            Dictionary<string, string> parameters) : base(logger, outputFilePath)
        {
            _templateManager = ObjectFactory.GetInstance<ITemplateManager>();
            _outputFilePath = outputFilePath;
            _templateFilePath = templateFilePath;
            _parameters = parameters;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var outputContent = _templateManager.GenerateDocument(_templateFilePath, _parameters);

            FileManager.Write(_outputFilePath, outputContent);
        }
    }
}
