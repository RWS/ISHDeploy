
namespace InfoShare.Deployment.Data.Managers.Interfaces
{
    public interface ITextConfigManager
    {
        void CommentBlock(string filePath, string searchPattern);

        void UncommentBlock(string filePath, string searchPattern);
    }
}
