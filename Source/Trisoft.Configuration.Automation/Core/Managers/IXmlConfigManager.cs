namespace Trisoft.Configuration.Automation.Core.Managers
{
    public interface IXmlConfigManager
    {
        void Backup();
        void CommentNode(string commentedLineContains);
        void UncommentNode(string commentedLineContains);
        bool IsNodeCommented(string commentedLineContains);
        void RestoreOriginal();
    }
}
