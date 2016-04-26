using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// Generates documents from resource templates.
    /// </summary>
    /// <seealso cref="ITemplateManager" />
    public class TemplateManager : ITemplateManager
    {
        /// <summary>
        /// The templates resource folder.
        /// </summary>
        private const string TemplateBaseFolder = "ISHDeploy.Resources.Templates";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TemplateManager(ILogger logger)
        {
            _logger = logger;
        }

        #region Template files        
        /// <summary>
        /// The CM security token service template
        /// </summary>
        public const string CMSecurityTokenServiceTemplate = "CM Security Token Service Requirements.md";

        #endregion

        /// <summary>
        /// Generates the CM security token service document.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="cmWebAppName">Name of the CM web application.</param>
        /// <param name="wsWebAppName">Name of the WS web application.</param>
        /// <param name="certificateFileName">Name of the certificate file.</param>
        /// <param name="certificateContent">Content of the certificate file.</param>
        /// <returns>Content of the generated document.</returns>
        public string GetCMSecurityTokenServiceDoc(string hostName, string cmWebAppName, string wsWebAppName, string certificateFileName, string certificateContent)
        {
            var parameters = new Dictionary<string, string>
            {
                {"$ishhostname", hostName},
                {"$ishcmwebappname", cmWebAppName},
                {"$ishwswebappname", wsWebAppName},
                {"$ishwscertificate", certificateFileName},
                {"$ishwscontent", certificateContent}
            };

            return GenerateDocument(CMSecurityTokenServiceTemplate, parameters);
        }

        /// <summary>
        /// Generate output document from the template.
        /// </summary>
        /// <param name="templateFileName">Name of the template file.</param>
        /// <param name="parameters">All parameters that need to be filled out in the template.</param>
        /// <returns></returns>
        private string GenerateDocument(string templateFileName, IDictionary<string, string> parameters)
        {
            string templateFile = $"{TemplateBaseFolder}.{templateFileName}";
            string templateContent;

            _logger.WriteDebug($"Reading the resource template: {templateFile}");
            using (var resourceReader = Assembly.GetExecutingAssembly().GetManifestResourceStream(templateFile))
            {
                using (var reader = new StreamReader(resourceReader))
                {
                    templateContent = reader.ReadToEnd();
                }
            }

            _logger.WriteDebug("Replacing all parameters in template: " + string.Join("; ", parameters.Select(param => $"{param.Key}={param.Value}").ToArray()));
            templateContent = parameters.Aggregate(templateContent, (current, parameter) => current.Replace(parameter.Key, parameter.Value));

            return templateContent;
        }
    }
}
