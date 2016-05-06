
namespace ISHDeploy.Data.Managers.Interfaces
{
    /// <summary>
    /// Generates documents from resource templates.
    /// </summary>
    public interface ITemplateManager
    {
        /// <summary>
        /// Generates the CM security token service document.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="cmWebAppName">Name of the CM web application.</param>
        /// <param name="wsWebAppName">Name of the WS web application.</param>
        /// <param name="certificateFileName">Name of the certificate file.</param>
        /// <param name="certificateContent">Content of the certificate file.</param>
        /// <returns>Content of the generated document.</returns>
        string GetCMSecurityTokenServiceDoc(string hostName, string cmWebAppName, string wsWebAppName, string certificateFileName, string certificateContent);
    }
}
