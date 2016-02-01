
namespace Trisoft.Configuration.Automation.Core.Managers
{
    public interface IXmlConfigManager
    {
        void Backup();
        void CommentNode(string commentPattern);
        void UncommentNode(string commentPattern);
        void RestoreOriginal();
    }
}
