using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateManager
    {
        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TemplateManager(ILogger logger)
        {
            _logger = logger;
            _fileManager = ObjectFactory.GetInstance<IFileManager>();
        }

        /// <summary>
        /// Saves the cm security token service.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cmId">The cm identifier.</param>
        /// <param name="wsIds">The ws ids.</param>
        /// <param name="serviceCertThumbprint">The service cert thumbprint.</param>
        public void SaveCMSecurityTokenService(string filePath, string cmId, string[] wsIds, string serviceCertThumbprint)
        {
            const string templateFile = "ISHDeploy.Resources.Templates.CM Security Token Service Requirements.md";
            var parameters = new Dictionary<string, string>
            {
                {"[token:CM_id]", cmId},
                {"[token:WS_ids]", string.Join(Environment.NewLine, wsIds)},
                {"[token:ServiceCertificateThumbprint]", serviceCertThumbprint}
            };

            SaveTemplate(filePath, templateFile, parameters);
        }

        /// <summary>
        /// Saves the template.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="templateFile">The template file.</param>
        /// <param name="parameters">The parameters.</param>
        private void SaveTemplate(string filePath, string templateFile, IDictionary<string, string> parameters)
        {
            string templateContent;
            using (var resourceReader = Assembly.GetExecutingAssembly().GetManifestResourceStream(templateFile))
            {
                using (var reader = new StreamReader(resourceReader))
                {
                    templateContent = reader.ReadToEnd();
                }
            }

            templateContent = parameters.Aggregate(templateContent, (current, parameter) => current.Replace(parameter.Key, parameter.Value));

            _fileManager.Write(filePath, templateContent);
        }
    }
}
