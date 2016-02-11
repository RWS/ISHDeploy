namespace InfoShare.Deployment.Data.Services
{
    public interface IXmlConfigManager
    {
        void Backup();
        void CommentBlock(string searchPattern);
        void CommentNode(string xpath);
        void UncommentBlock(string searchPattern);
        void UncommentNode(string searchPattern);
        void RestoreOriginal();
        void SetAttributeValue(string filePath, string xpath, string attributeName, string value);
    }
}
