using System.Collections.Generic;

namespace InfoShare.Deployment.Data.Managers.Interfaces
{
    public interface IXmlConfigManager
    {
        void CommentBlock(string filePath, string searchPattern);
        void CommentNode(string filePath, string xpath);
        Dictionary<string, string> GetAllInstallParamsValues(string filePath);
        void UncommentBlock(string filePath, string searchPattern);
        void UncommentNode(string filePath, string searchPattern);
        void SetAttributeValue(string filePath, string xpath, string attributeName, string value);
    }
}
