using System.Collections.Generic;

namespace InfoShare.Deployment.Data.Services
{
    public interface IXmlConfigManager
    {
        Dictionary<string, string> GetAllInstallParamsValues(string filePath);
        void CommentNode(string filePath, string commentPattern);
        void UncommentNode(string filePath, string commentPattern);
    }
}
