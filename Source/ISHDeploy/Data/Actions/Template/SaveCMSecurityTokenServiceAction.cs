using System.Collections.Generic;
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
        /// The CM security token service template
        /// </summary>
        public const string CMSecurityTokenServiceTemplate = "CM Security Token Service Requirements.md";

        /// <summary>
        /// The output file path.
        /// </summary>
        private readonly string _outputFilePath;

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
        /// The certificate file name.
        /// </summary>
        private readonly string _certificateFileName;

        /// <summary>
        /// The certificate content.
        /// </summary>
        private readonly string _certificateContent;

        /// <summary>
        /// The template manager
        /// </summary>
        private readonly ITemplateManager _templateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveCMSecurityTokenServiceAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="outputFilePath">The output file path.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="cmWebAppName">Name of the CM web application.</param>
        /// <param name="wsWebAppname">The WS web appname.</param>
        /// <param name="certificateFileName">Name of the certificate file.</param>
        /// <param name="certificateContent">Content of the certificate file.</param>
        public SaveCMSecurityTokenServiceAction(ILogger logger,
            string outputFilePath,
            string hostname,
            string cmWebAppName,
            string wsWebAppname,
            string certificateFileName,
            string certificateContent) : base(logger, outputFilePath)
        {
            _templateManager = ObjectFactory.GetInstance<ITemplateManager>();
            _outputFilePath = outputFilePath;
            _hostName = hostname;
            _cmWebAppName = cmWebAppName;
            _wsWebAppName = wsWebAppname;
            _certificateFileName = certificateFileName;
            _certificateContent = certificateContent;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            var parameters = new Dictionary<string, string>
            {
                {"$ishhostname", _hostName},
                {"$ishcmwebappname", _cmWebAppName},
                {"$ishwswebappname", _wsWebAppName},
                {"$ishwscertificate", _certificateFileName},
                {"$ishwscontent", _certificateContent}
            };

            var outputContent = _templateManager.GenerateDocument(CMSecurityTokenServiceTemplate, parameters);

            FileManager.Write(_outputFilePath, outputContent);
        }
    }
}
