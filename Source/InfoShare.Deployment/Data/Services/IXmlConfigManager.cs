namespace InfoShare.Deployment.Data.Services
{
    public interface IXmlConfigManager
    {
        void Backup();
        void CommentNode(string commentPattern);
        void UncommentNode(string commentPattern);
        void RestoreOriginal();
    }
}
