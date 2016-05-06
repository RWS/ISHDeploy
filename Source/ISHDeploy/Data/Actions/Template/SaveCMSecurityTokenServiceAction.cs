using System.IO;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Template
{
    /// <summary>
    /// Saves CMSecurityTokenService document generated from resource template.
    /// </summary>
    /// <seealso cref="SingleFileCreationAction" />
    public class SaveCMSecurityTokenServiceAction : SingleFileCreationAction
    {
        /// <summary>
        /// The output file path.
        /// </summary>
        private readonly string _outputFilePath;

        /// <summary>
        /// The certificate file path.
        /// </summary>
        private readonly string _certificateFilePath;

        /// <summary>
        /// The CM web application name.
        /// </summary>
        private readonly string _cmWebAppName;

        /// <summary>
        /// The host name.
        /// </summary>
        private readonly string _hostName;

        /// <summary>
        /// The WS web application name.
        /// </summary>
        private readonly string _wsWebAppName;

        /// <summary>
        /// The template manager
        /// </summary>
        private readonly ITemplateManager _templateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveCMSecurityTokenServiceAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="certificateFilePath">The certificate file path.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="cmWebAppName">Name of the CM web application.</param>
        /// <param name="wsWebAppname">The WS web appname.</param>
        public SaveCMSecurityTokenServiceAction(ILogger logger,
            string outputFilePath,
            string certificateFilePath,
            string hostname,
            string cmWebAppName,
            string wsWebAppname) : base(logger, outputFilePath)
        {
            _templateManager = ObjectFactory.GetInstance<ITemplateManager>();
            _outputFilePath = outputFilePath;
            _certificateFilePath = certificateFilePath;
            _hostName = hostname;
            _cmWebAppName = cmWebAppName;
            _wsWebAppName = wsWebAppname;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var certificateContent = _fileManager.ReadAllText(_certificateFilePath);
            var certificateFileName = Path.GetFileName(_certificateFilePath);

            var outputContent = _templateManager.GetCMSecurityTokenServiceDoc(_hostName, _cmWebAppName, _wsWebAppName, certificateFileName, certificateContent);

            var outputFolder = Path.GetDirectoryName(_outputFilePath);
            if (!_fileManager.FolderExists(outputFolder))
            {
                _fileManager.CreateDirectory(outputFolder);
            }

            _fileManager.Write(_outputFilePath, outputContent);
        }
    }
}
