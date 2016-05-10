using System.Collections.Generic;

namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Generates documents from resource templates.
    /// </summary>
    public interface ITemplateManager
    {
        /// <summary>
        /// Generate output document from the template.
        /// </summary>
        /// <param name="templateFileName">Name of the template file.</param>
        /// <param name="parameters">All parameters that need to be filled out in the template.</param>
        /// <returns></returns>
        string GenerateDocument(string templateFileName, IDictionary<string, string> parameters);
    }
}
