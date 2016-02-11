namespace InfoShare.Deployment.Data.Services
{
    public interface IXmlConfigManager
    {
        void CommentNode(string filePath, string commentPattern);
        void UncommentNode(string filePath, string commentPattern);
    }
}
